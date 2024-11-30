namespace App.Server.Notification.Tests.Unit.Tests;

/// <summary>
/// Tests for <see cref="Result"/>.
/// </summary>
[Collection("Result test collection")]
public class ResultTest
{
    [Fact]
    public void Ok_Result_Should_Have_No_Errors()
    {
        // Arrange
        var result = Result.Ok();

        // Act
        var errors = result.Errors;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(errors);
    }

    [Fact]
    public void Failed_Result_Should_Have_Empty_Error()
    {
        // Arrange
        var result = Result.Fail();

        // Act
        var errors = result.Errors;

        // Assert
        Assert.False(result.IsSuccess);
        var error = Assert.Single(errors);
        Assert.Equivalent(Error.Empty, error);
    }

    [Fact]
    public void Failed_Result_Should_Have_One_Error()
    {
        // Arrange
        var result1 = Result.Fail("Error message");
        var result2 = Result.Fail(new Error("Error message"));
        var result3 = Result.Fail(new List<IError> { new Error("Error message") });
        var result4 = Result.Fail("Error message", ("Source", "Right here"));
        var result5 = Result.Fail(
            "Error message",
            new Dictionary<string, object>() { { "Source", "Right here" } }
        );
        var result6 = Result.Fail();

        // Act
        List<Result> results = [result1, result2, result3, result4, result5, result6];

        // Assert
        foreach (var result in results)
        {
            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
        }
    }

    [Fact]
    public void Failed_Result_Should_Have_One_Error_With_Metadata()
    {
        const string sourceKey = "Source";
        const string source = "Right here";
        const string message = "Error message";

        // Arrange
        var result1 = Result.Fail(message, (sourceKey, source));
        var result2 = Result.Fail(
            message,
            new Dictionary<string, object>() { { sourceKey, source } }
        );

        // Act
        List<Result> results = [result1, result2];

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
    public void Failed_Result_Should_Have_One_Error_With_Multiple_Metadata()
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

        var result = Result.Fail(message, errorDictionary);

        // Act

        // Assert
        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Equal(3, error.Metadata.Count);
        Assert.Equal(message, error.Message);
        Assert.Equal(source, error.Metadata[sourceKey]);
    }

    [Fact]
    public void Failed_Result_Should_Have_Multiple_Errors()
    {
        // Arrange
        List<IError> errors =
        [
            new Error("Error message 1"),
            new Error("Error message 2"),
            new Error("Error message 3"),
        ];
        var result = Result.Fail(errors);

        // Act

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(3, result.Errors.Count);
    }

    [Fact]
    public void Failed_Result_Should_Have_Multiple_Errors_With_Multiple_Metadata()
    {
        // Arrange
        const string sourceKey = "Source";
        const string source = "Right here";
        const string message = "Error message";

        var result1 = Result.Fail([new Error(message), new Error(message)]);
        var result2 = Result.Fail(
            [new Error(message, (sourceKey, source)), new Error(message, ($"{sourceKey}2", source))]
        );
        var result3 = Result.Fail(
            [
                new Error(message, new Dictionary<string, object>() { { sourceKey, source } }),
                new Error(
                    message,
                    new Dictionary<string, object>() { { $"{sourceKey}2", source } }
                ),
            ]
        );

        // Act
        List<Result> results = [result1, result2, result3];

        // Assert
        foreach (var result in results)
        {
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Errors.Count);
        }
    }

    [Fact]
    public void Failed_TypedResult_Should_Implicitly_Cast_To_Errors()
    {
        // Arrange
        const string message = "Error message";

        var result = Result.Fail(message);

        // Act
        ImmutableList<IError> values = result;

        // Assert
        Assert.False(result.IsSuccess);
        var value = Assert.Single(values);
        Assert.Equal(message, value.Message);
    }

    [Fact]
    public void Ok_Result_Should_Deconstruct()
    {
        // Arrange
        var result = Result.Ok();

        // Act
        var (isSuccess, errors) = result;

        // Assert
        Assert.True(isSuccess);
        Assert.Empty(errors);
    }

    [Fact]
    public void Failed_Result_Should_Deconstruct()
    {
        // Arrange
        var result = Result.Fail("Error message");

        // Act
        var (isSuccess, errors) = result;

        // Assert
        Assert.False(isSuccess);
        Assert.Single(errors);
    }

    [Fact]
    public void Failed_Result_Should_Implicitly_Cast_To_Result()
    {
        // Arrange
        Result result1 = new Error("Error message");
        Result result2 = new List<IError>() { new Error("Error message") };

        // Act
        List<Result> results = [result1, result2];

        // Assert
        foreach (var result in results)
        {
            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
        }
    }

    [Fact]
    public void Failed_Result_Should_Cast_To_Result()
    {
        // Arrange
        var result1 = (Result)new Error("Error message");
        var result2 = (Result)new List<IError>() { new Error("Error message") };
        var result3 = (Result)new HashSet<IError>() { new Error("Error message") };
        var result4 = (Result)new List<IError>() { new Error("Error message") }.ToImmutableList();
        // Act
        List<Result> results = [result1, result2, result3, result4];

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
        var result = Result.Ok();

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
        var result = Result.Ok();

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
            Assert.Contains(string.Empty, contents);
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
        var result = Result.Fail();

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
        var result = Result.Fail();

        try
        {
            // Act
            Log.Logger = new LoggerConfiguration().WriteTo.File(fileName).CreateLogger();

            result.LogIfSuccess(
                LogEventLevel.Information,
                nameof(Failed_TypedResult_Should_Not_Log_On_Success),
                logText
            );

            Log.CloseAndFlush();

            using var streamReader = new StreamReader(fileName);
            var contents = streamReader.ReadToEnd();

            // Assert
            Assert.Contains(string.Empty, contents);
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
        var result = Result.Fail(Error.Empty);

        try
        {
            // Act
            Log.Logger = new LoggerConfiguration().WriteTo.File(fileName).CreateLogger();

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
        const string logTemplate = "Logged a message: {Message} with errors: {Errors}";
        const string message = "Random message";
        const string errorMessage = "Error message";
        const string fileName = $"{nameof(TypedResult_Logs_Should_Contain_Result_Values)}.txt";
        var result = Result.Fail(new Error(errorMessage));

        try
        {
            // Act
            Log.Logger = new LoggerConfiguration().WriteTo.File(fileName).CreateLogger();

            result.Log(
                LogEventLevel.Information,
                nameof(TypedResult_Logs_Should_Contain_Result_Values),
                logTemplate,
                static r => [message, r.Errors[0].Message]
            );

            Log.CloseAndFlush();

            using var streamReader = new StreamReader(fileName);
            var contents = streamReader.ReadToEnd();

            // Assert
            Assert.Contains(message, contents);
            Assert.Contains(errorMessage, contents);
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
        var result = Result.Ok();

        // Act

        var resultString = result.ToString();

        // Assert
        Assert.Contains("IsSuccess = True", resultString);
    }

    [Fact]
    public void Failed_TypedResult_Contains_Error()
    {
        // Arrange
        var result1 = Result.Fail(new Error("Error message"));
        var result2 = Result.Fail(new TestError("Error message"));

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
        var result1 = Result.Fail(new Error("Error message"));
        var result2 = Result.Fail(new TestError("Error message"));

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
        var number = 1;

        // Act
        var result = Result.Try(() => number++);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, number);
    }

    [Fact]
    public void Try_Should_Return_Failed_Result()
    {
        // Arrange
        const string message = "Error message";

        // Act
        var result = Result.Try(static () => throw new ArithmeticException(message));

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
        var catchHandler = new Func<Exception, IError>(static e => new TestError(e.Message));

        // Act
        var result = Result.Try(static () => throw new ArithmeticException(message), catchHandler);

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
        var number = 1;

        // Act
        var result = await Result.TryAsync(() => Task.Run(() => number++));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, number);
    }

    [Fact]
    public async Task TryAsync_Should_Return_Failed_Result()
    {
        // Arrange
        const string message = "Error message";

        // Act
        var result = await Result.TryAsync(static () =>
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
        var result = await Result.TryAsync(static () => ValueTask.CompletedTask);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task TryAsync_ValueTask_Should_Return_Failed_Result()
    {
        // Arrange
        const string message = "Error message";

        // Act
        var result = await Result.TryAsync(static () =>
        {
            throw new ArithmeticException(message);

#pragma warning disable CS0162 // Unreachable code detected
            return ValueTask.CompletedTask;
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
        var result = await Result.TryAsync(
            static () =>
            {
                throw new ArithmeticException(message);

#pragma warning disable CS0162 // Unreachable code detected
                return ValueTask.CompletedTask;
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
