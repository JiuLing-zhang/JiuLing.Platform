namespace JiuLing.Platform.Models;

public class ChatContext(string prompt, List<ChatMessage>? chatMessages)
{
    public string Prompt { get; set; } = prompt;
    public List<ChatMessage>? ChatMessages { get; set; } = chatMessages;
}

public class ChatMessage(string role, string content)
{
    public string Role { get; set; } = role;

    public string Content { get; set; } = content;
}