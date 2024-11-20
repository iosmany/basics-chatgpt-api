namespace ChatgptSdk.Lib;

public interface IChatGptSdk : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Send a message to ChatGPT and get a response.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    Task<string> SendMessageAsync(string message);
    /// <summary>
    /// Stream a conversation with ChatGPT.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task StreamConversationAsync(string message);
}
