namespace CSharpExtensions.Lib.Railway.Contexts;

public record ActionResultProfileSuccessContext(Result Result);

public record ActionResultProfileSuccessContext<T>(Result<T> Result);

