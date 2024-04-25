using Mastonet;

using Newtonsoft.Json;



namespace Limerick
{
    public class Mastodon
    {
        private MastodonClient Login()
        {
            var secrets = JsonConvert.DeserializeObject<Secrets>(File.ReadAllText("secrets.json")) ?? throw new Exception("cannot read secrets");
            return new MastodonClient(secrets.Instance, secrets.AccessToken);
        }

        public async Task SendArticleToMastodon(Article article)
        {
            try
            {
                var client = Login();
                var mainEntry = await client.PublishStatus(article.Title, Visibility.Public);
                if (!string.IsNullOrEmpty(mainEntry.Id))
                {
                    var reply = await client.PublishStatus(article.Url, Visibility.Private, mainEntry.Id, spoilerText: "Guardian Article");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}