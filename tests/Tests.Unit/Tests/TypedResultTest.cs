namespace App.Server.Notification.Tests.Unit.Tests;

/// <summary>
/// Tests for <see cref="Result{TValue}"/>.
/// </summary>
[Collection("Result test collection")]
public class TypedResultTest
{
    [Fact]
    public void Ok_TypedResult_Should_Have_No_Errors()
    {
        // Arrange
        var result = Result<int>.Ok(1);

        // Act
        var errors = result.Errors;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(errors);
    }

    [Fact]
    public void Failed_TypedResult_Should_Have_Empty_Error()
    {
        // Arrange
        var result = Result<int>.Fail();

        // Act
        var errors = result.Errors;

        // Assert
        Assert.False(result.IsSuccess);
        var error = Assert.Single(errors);
        Assert.Equivalent(Error.Empty, error);
    }

    [Fact]
    public void Failed_TypedResult_Should_Have_One_Error()
    {
        // Arrange
        var result1 = Result<int>.Fail("Error message");
        var result2 = Result<int>.Fail(new Error("Error message"));
        var result3 = Result<int>.Fail([new Error("Error message")]);
        var result4 = Result<int>.Fail("Error message", ("Source", "Right here"));
        var result5 = Result<int>.Fail(
            "Error message",
            new Dictionary<string, object>() { { "Source", "Right here" } }
        );
        var result6 = Result<int>.Fail();

        // Act
        List<Result<int>> results = [result1, result2, result3, result4, result5, result6];

        // Assert
        foreach (var result in results)
        {
            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
        }
    }

    [Fact]
    public void Failed_TypedResult_Should_Have_One_Error_With_Metadata()
    {
        const string sourceKey = "Source";
        const string source = "Right here";
        const string message = "Error message";

        // Arrange
        var result1 = Result<int>.Fail(message, (sourceKey, source));
        var result2 = Result<int>.Fail(
            message,
            new Dictionary<string, object>() { { sourceKey, source } }
        );

        // Act
        List<Result<int>> results = [result1, result2];

        // Assert
        foreach (var result in results)
        {
            Assert.False(result.IsSuccess);
            var error = Assert.Single(result.Errors);
            Assert.Equal(message, error.Message);
            Assert.Equal(source, error.Metadata[sourceKey]);
        }
    }

    [Fact]
    public void Failed_TypedResult_Should_Have_One_Error_With_Multiple_Metadata()
    {
        // Arrange
        const string sourceKey = "Source";
        const string source = "Right here";
        const string message = "Error message";

        var errorDictionary = new Dictionary<string, object>()
        {
            { sourceKey, source },
            { $"{sourceKey}2", source },
            { $"{sourceKey}3", source },
        };

        var result = Result<int>.Fail(message, errorDictionary);

        // Act

        // Assert
        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Equal(3, error.Metadata.Count);
        Assert.Equal(message, error.Message);
        Assert.Equal(source, error.Metadata[sourceKey]);
    }

    [Fact]
    public void Failed_TypedResult_Should_Have_Multiple_Errors()
    {
        // Arrange
        List<IError> errors =
        [
            new Error("Error message 1"),
            new Error("Error message 2"),
            new Error("Error message 3"),
        ];
        var result = Result<int>.Fail(errors);

        // Act

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(3, result.Errors.Count);
    }

    [Fact]
    public void Failed_TypedResult_Should_Have_Multiple_Errors_With_Multiple_Metadata()
    {
        // Arrange
        const string sourceKey = "Source";
        const string source = "Right here";
        const string message = "Error message";

        var result1 = Result<int>.Fail([new Error(message), new Error(message)]);
        var result2 = Result<int>.Fail(
            [new Error(message, (sourceKey, source)), new Error(message, ($"{sourceKey}2", source))]
        );
        var result3 = Result<int>.Fail(
            [
                new Error(message, new Dictionary<string, object>() { { sourceKey, source } }),
                new Error(
                    message,
                    new Dictionary<string, object>() { { $"{sourceKey}2", source } }
                ),
            ]
        );

        // Act
        List<Result<int>> results = [result1, result2, result3];

        // Assert
        foreach (var result in results)
        {
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Errors.Count);
        }
    }

    [Fact]
    public void Ok_TypedResult_Should_Deconstruct()
    {
        // Arrange
        var result = Result<int>.Ok(1);

        // Act
        var (isSuccess, value, errors) = result;

        // Assert
        Assert.True(isSuccess);
        Assert.Equal(1, value);
        Assert.Empty(errors);
    }

    [Fact]
    public void Failed_TypedResult_Should_Deconstruct()
    {
        // Arrange
        var result = Result<int>.Fail("Error message");

        // Act
        var (isSuccess, value, errors) = result;

        // Assert
        Assert.False(isSuccess);
        Assert.Equal(default, value);
        Assert.Single(errors);
    }

    [Fact]
    public void Ok_TypedResult_Should_Implicitly_Cast_To_Result()
    {
        // Arrange
        Result<string> result1 = "Success";
        Result<object> result2 = new { Prop1 = 1, Prop2 = "2" };
        Result<TestRecord> result3 = new TestRecord(1, "2");

        // Act
        List<IResult<object>> results = [result1, result2, result3];

        // Assert
        foreach (var result in results)
        {
            Assert.True(result.IsSuccess);
            Assert.NotEqual(default, result.Value);
            Assert.Empty(result.Errors);
        }
    }

    [Fact]
    public void Failed_TypedResult_Should_Implicitly_Cast_To_Result()
    {
        // Arrange
        Result<int> result1 = new Error("Error message");
        Result<int> result2 = new List<IError>() { new Error("Error message") };

        // Act
        List<Result<int>> results = [result1, result2];

        // Assert
        foreach (var result in results)
        {
            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
        }
    }

    [Fact]
    public void Ok_TypedResult_Should_Implicitly_Cast_To_Value()
    {
        // Arrange
        const string message = "Success";

        var result = Result<string>.Ok(message);

        // Act
        string value = result;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(message, value);
    }

    [Fact]
    public void Failed_TypedResult_Should_Implicitly_Cast_To_Errors()
    {
        // Arrange
        const string message = "Error message";

        var result = Result<string>.Fail(message);

        // Act
        ImmutableList<IError> values = result;

        // Assert
        Assert.False(result.IsSuccess);
        var value = Assert.Single(values);
        Assert.Equal(message, value.Message);
    }

    [Fact]
    public void Failed_TypedResult_Should_Throw_When_Attempting_To_Implicitly_Cast_To_Value()
    {
        // Arrange
        const string message = "Error message";

        var result = Result<string>.Fail(message);

        // Act

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Throws<ResultFailedException>(() =>
        {
            // Implicit cast should fail
            string _ = result;
        });
    }

    [Fact]
    public void Ok_TypedResult_Should_Cast_To_Result()
    {
        // Arrange
        var result1 = (Result<string>)"Success";
        var result2 = (Result<object>)new { Prop1 = 1, Prop2 = "2" };
        var result3 = (Result<TestRecord>)new TestRecord(1, "2");

        // Act
        List<IResult<object>> results = [result1, result2, result3];

        // Assert
        foreach (var result in results)
        {
            Assert.True(result.IsSuccess);
            Assert.NotEqual(default, result.Value);
            Assert.Empty(result.Errors);
        }
    }

    [Fact]
    public void Failed_TypedResult_Should_Cast_To_Result()
    {
        // Arrange
        var result1 = (Result<int>)new Error("Error message");
        var result2 = (Result<int>)new List<IError>() { new Error("Error message") };
        var result3 = (Result<int>)new HashSet<IError>() { new Error("Error message") };
        var result4 =
            (Result<int>)new List<IError>() { new Error("Error message") }.ToImmutableList();

        // Act
        List<Result<int>> results = [result1, result2, result3, result4];

        // Assert
        foreach (var result in results)
        {
            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
        }
    }

    [Fact]
    public void Ok_TypedResult_Should_Log_On_Success()
    {
        // Arrange
        const string logText = "Logged a message";
        const string fileName = $"{nameof(Ok_TypedResult_Should_Log_On_Success)}.txt";
        var result = Result<int>.Ok(1);

        try
        {
            // Act
            Log.Logger = new LoggerConfiguration().WriteTo.File(fileName).CreateLogger();

            result.LogIfSuccess(
                LogEventLevel.Information,
                nameof(Ok_TypedResult_Should_Log_On_Success),
                logText
            );

            Log.CloseAndFlush();

            using var streamReader = new StreamReader(fileName);
            var contents = streamReader.ReadToEnd();

            // Assert
            Assert.Contains(logText, contents);
        }
        finally
        {
            // Cleanup
            File.Delete(fileName);
        }
    }

    [Fact]
    public void Ok_TypedResult_Should_Not_Log_On_Failure()
    {
        // Arrange
        const string logText = "Logged a message";
        const string fileName = $"{nameof(Ok_TypedResult_Should_Not_Log_On_Failure)}.txt";
        var result = Result<int>.Ok(1);

        try
        {
            // Act
            Log.Logger = new LoggerConfiguration().WriteTo.File(fileName).CreateLogger();

            result.LogIfFailure(
                LogEventLevel.Information,
                nameof(Ok_TypedResult_Should_Not_Log_On_Failure),
                logText
            );

            Log.CloseAndFlush();

            using var streamReader = new StreamReader(fileName);
            var contents = streamReader.ReadToEnd();

            // Assert
            Assert.Equal(string.Empty, contents);
        }
        finally
        {
            // Cleanup
            File.Delete(fileName);
        }
    }

    [Fact]
    public void Failed_TypedResult_Should_Log_On_Failure()
    {
        // Arrange
        const string logText = "Logged a message";
        const string fileName = $"{nameof(Failed_TypedResult_Should_Log_On_Failure)}.txt";
        var result = Result<int>.Fail();

        try
        {
            // Act
            Log.Logger = new LoggerConfiguration().WriteTo.File(fileName).CreateLogger();

            result.LogIfFailure(
                LogEventLevel.Information,
                nameof(Failed_TypedResult_Should_Log_On_Failure),
                logText
            );

            Log.CloseAndFlush();

            using var streamReader = new StreamReader(fileName);
            var contents = streamReader.ReadToEnd();

            // Assert
            Assert.Contains(logText, contents);
        }
        finally
        {
            // Cleanup
            File.Delete(fileName);
        }
    }

    [Fact]
    public void Failed_TypedResult_Should_Not_Log_On_Success()
    {
        // Arrange
        const string logText = "Logged a message";
        const string fileName = $"{nameof(Failed_TypedResult_Should_Not_Log_On_Success)}.txt";
        var result = Result<int>.Fail();

        try
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File(fileName).CreateLogger();

            // Act
            result.LogIfSuccess(
                LogEventLevel.Information,
                nameof(Failed_TypedResult_Should_Not_Log_On_Success),
                logText
            );

            Log.CloseAndFlush();

            using var streamReader = new StreamReader(fileName);
            var contents = streamReader.ReadToEnd();

            // Assert
            Assert.Equal(string.Empty, contents);
        }
        finally
        {
            // Cleanup
            File.Delete(fileName);
        }
    }

    [Fact]
    public void TypedResult_Logs_Should_Contain_Values()
    {
        // Arrange
        const string logTemplate = "Logged a message: {Message}";
        const string message = "Random message";
        const string fileName = $"{nameof(TypedResult_Logs_Should_Contain_Values)}.txt";

        try
        {
            var result = Result<int>.Ok(1);
            Log.Logger = new LoggerConfiguration().WriteTo.File(fileName).CreateLogger();

            // Act
            result.Log(
                LogEventLevel.Information,
                nameof(TypedResult_Logs_Should_Contain_Values),
                logTemplate,
                message
            );

            Log.CloseAndFlush();

            using var streamReader = new StreamReader(fileName);
            var contents = streamReader.ReadToEnd();

            // Assert
            Assert.Contains(message, contents);
        }
        finally
        {
            // Cleanup
            File.Delete(fileName);
        }
    }

    [Fact]
    public void TypedResult_Logs_Should_Contain_Result_Values()
    {
        // Arrange
        const string logTemplate = "Logged a message: {Message} with value: {Value}";
        const string message = "Random message";
        const string fileName = $"{nameof(TypedResult_Logs_Should_Contain_Result_Values)}.txt";
        var result = Result<int>.Ok(1);

        try
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File(fileName).CreateLogger();

            // Act
            result.Log(
                LogEventLevel.Information,
                nameof(TypedResult_Logs_Should_Contain_Result_Values),
                logTemplate,
                r => [message, r.Value]
            );

            Log.CloseAndFlush();

            using var streamReader = new StreamReader(fileName);
            var contents = streamReader.ReadToEnd();

            // Assert
            Assert.Contains(message, contents);
            Assert.Contains(result.Value.ToString(), contents);
        }
        finally
        {
            // Cleanup
            File.Delete(fileName);
        }
    }

    [Fact]
    public void Ok_TypedResult_Should_Get_Value_From_To_String()
    {
        // Arrange
        const int value = 1;
        var result = Result<int>.Ok(value);

        // Act
        var resultString = result.ToString();

        // Assert
        Assert.Contains("IsSuccess = True", resultString);
        Assert.Contains(value.ToString(), resultString);
    }

    [Fact]
    public void Failed_TypedResult_Should_Get_One_Error_From_To_String()
    {
        // Arrange
        const string message = "Error message";
        var result = Result<int>.Fail(message);

        // Act
        var resultString = result.ToString();

        // Assert
        Assert.Contains("IsSuccess = False", resultString);
        Assert.Contains(message, resultString);
    }

    [Fact]
    public void Failed_TypedResult_Should_Get_Multiple_Errors_From_To_String()
    {
        // Arrange
        const string message = "Error message";
        var errors = new List<IError>() { new Error(message), new Error(message) };

        var result = Result<int>.Fail(errors);

        // Act
        var resultString = result.ToString();

        // Assert
        Assert.Contains("IsSuccess = False", resultString);
        Assert.Contains(message, resultString);
    }

    [Fact]
    public void Failed_TypedResult_Contains_Error()
    {
        // Arrange
        var result1 = Result<int>.Fail(new Error("Error message"));
        var result2 = Result<int>.Fail(new TestError("Error message"));

        // Assert

        // Assert
        Assert.True(result1.HasError<Error>());
        Assert.True(result1.HasError(typeof(Error)));
        Assert.True(result2.HasError<TestError>());
        Assert.True(result2.HasError(typeof(TestError)));
    }

    [Fact]
    public void Failed_TypedResult_Does_Not_Contain_Error()
    {
        // Arrange
        var result1 = Result<int>.Fail(new Error("Error message"));
        var result2 = Result<int>.Fail(new TestError("Error message"));

        // Assert

        // Assert
        Assert.False(result2.HasError<Error>());
        Assert.False(result2.HasError(typeof(Error)));
        Assert.False(result1.HasError<TestError>());
        Assert.False(result1.HasError(typeof(TestError)));
    }

    [Fact]
    public void Try_Should_Return_Successful_Result()
    {
        // Arrange

        // Act
        var result = Result<int>.Try(() => 1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Try_Should_Return_Failed_Result()
    {
        // Arrange
        const string message = "Error message";

        // Act
        var result = Result<int>.Try(() => throw new ArithmeticException(message));

        // Assert
        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Equal(message, error.Message);
    }

    [Fact]
    public void Try_Should_Return_Failed_Result_With_Different_Error()
    {
        // Arrange
        const string message = "Error message";
        var catchHandler = new Func<Exception, IError>(e => new TestError(e.Message));

        // Act
        var result = Result<int>.Try(() => throw new ArithmeticException(message), catchHandler);

        // Assert
        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Equal(message, error.Message);
        Assert.False(result.HasError<Error>());
        Assert.True(result.HasError<TestError>());
    }

    [Fact]
    public async Task TryAsync_Should_Return_Successful_Result()
    {
        // Arrange

        // Act
        var result = await Result<int>.TryAsync(static () => Task.FromResult(1));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public async Task TryAsync_Should_Return_Failed_Result()
    {
        // Arrange
        const string message = "Error message";

        // Act
        var result = await Result<int>.TryAsync(static () =>
        {
            throw new ArithmeticException(message);

#pragma warning disable CS0162 // Unreachable code detected
            return Task.FromResult(1);
#pragma warning restore CS0162 // Unreachable code detected
        });

        // Assert
        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Equal(message, error.Message);
    }

    [Fact]
    public async Task TryAsync_Should_Return_Failed_Result_With_Different_Error()
    {
        // Arrange
        const string message = "Error message";
        var catchHandler = new Func<Exception, IError>(static e => new TestError(e.Message));

        // Act
        var result = await Result<int>.TryAsync(
            static () =>
            {
                throw new ArithmeticException(message);

#pragma warning disable CS0162 // Unreachable code detected
                return Task.FromResult(1);
#pragma warning restore CS0162 // Unreachable code detected
            },
            catchHandler
        );

        // Assert
        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Equal(message, error.Message);
        Assert.False(result.HasError<Error>());
        Assert.True(result.HasError<TestError>());
    }

    [Fact]
    public async Task TryAsync_ValueTask_Should_Return_Successful_Result()
    {
        // Arrange

        // Act
        var result = await Result<int>.TryAsync(static () => ValueTask.FromResult(1));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public async Task TryAsync_ValueTask_Should_Return_Failed_Result()
    {
        // Arrange
        const string message = "Error message";

        // Act
        var result = await Result<int>.TryAsync(static () =>
        {
            throw new ArithmeticException(message);

#pragma warning disable CS0162 // Unreachable code detected
            return ValueTask.FromResult(1);
#pragma warning restore CS0162 // Unreachable code detected
        });

        // Assert
        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Equal(message, error.Message);
    }

    [Fact]
    public async Task TryAsync_ValueTask_Should_Return_Failed_Result_With_Different_Error()
    {
        // Arrange
        const string message = "Error message";
        var catchHandler = new Func<Exception, IError>(static e => new TestError(e.Message));

        // Act
        var result = await Result<int>.TryAsync(
            static () =>
            {
                throw new ArithmeticException(message);

#pragma warning disable CS0162 // Unreachable code detected
                return ValueTask.FromResult(1);
#pragma warning restore CS0162 // Unreachable code detected
            },
            catchHandler
        );

        // Assert
        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Equal(message, error.Message);
        Assert.False(result.HasError<Error>());
        Assert.True(result.HasError<TestError>());
    }
}
