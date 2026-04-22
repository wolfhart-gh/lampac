using Jint;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Models.SISI.Base;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdultJS
{
    public class ApiController : BaseSisiController
    {
        Engine engine = new Engine();

        public ApiController() : base(ModInit.conf)
        {
            engine.Execute(ModInit.jsFile);
        }

        [HttpGet]
        [Route("porngram")]
        async public Task<ActionResult> Index(string search, string sort, int pg = 1)
        {
            if (await IsRequestBlocked(rch: false))
                return badInitMsg;

        rhubFallback:
            var cache = await InvokeCacheResult<Channel>($"PornGram:{search}:{sort}:{pg}", 5, async e =>
            {
                string uri = engine.Invoke("BuildUrl", host, search, sort, pg).AsString();
                string html = await httpHydra.Get(uri);
                if (html == null)
                    return e.Fail("html", refresh_proxy: true);

                string json = engine.Invoke("Playlist", html, host, search, sort).AsString();

                var channel = JsonSerializer.Deserialize<Channel>(json);
                if (channel?.list == null || channel.list.Count == 0)
                    return e.Fail("playlists");

                return e.Success(channel);
            });

            if (IsRhubFallback(cache))
                goto rhubFallback;

            return PlaylistResult(cache);
        }


        [HttpGet]
        [Route("porngram/video")]
        async public Task<ActionResult> Video(string href)
        {
            if (await IsRequestBlocked(rch: false))
                return badInitMsg;

        rhubFallback:
            var cache = await InvokeCacheResult($"PornGram:{href}", 10, jsonContext.DictionaryStringString, async e =>
            {
                string html = await httpHydra.Get(href);
                if (html == null)
                    return e.Fail("html", refresh_proxy: true);

                string json = engine.Invoke("Video", html).AsString();
                return e.Success(JsonSerializer.Deserialize<Dictionary<string, string>>(json));
            });

            if (IsRhubFallback(cache))
                goto rhubFallback;

            return OnResult(cache);
        }
    }
}
