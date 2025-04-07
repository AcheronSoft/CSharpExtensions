namespace CSharpExtensions.Lib.Helpers;

public class LockAsync
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public async Task Lock(Func<Task> func)
    {
        await _semaphore.WaitAsync();

        try
        {
            await func();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<T> Lock<T>(Func<Task<T>> func)
    {
        await _semaphore.WaitAsync();

        try
        {
            return await func();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}