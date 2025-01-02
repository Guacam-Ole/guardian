using Limerick;

DateTimeOffset? _lastMessage = null;
var _guardianRss = new Rss();
var _openAI = new OpenAIGen();
var _mastodon = new Mastodon();

while (true)
{
    Console.WriteLine($"Getting news since '{_lastMessage}'");

    var messages = await _guardianRss.GetArticlesSince(_lastMessage);
    foreach (var message in messages)
    {
        if (string.IsNullOrEmpty(message.Url) || message.Url.Contains("/live/"))
        {
            Console.WriteLine($"Ignoring article '{message.Title}' because it has a live-url");
            continue;
        }
        if (string.IsNullOrEmpty(message.Title)) continue;
            
        message.Title = await _openAI.GetLimerickFromTitle(message.Title);
        if (message.Title != null) await _mastodon.SendArticleToMastodon(message);
    }

    if (messages.Any()) _lastMessage = messages.Max(q => q.Date);

    Thread.Sleep(TimeSpan.FromMinutes(15));
}