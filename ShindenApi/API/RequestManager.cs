using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shinden.Extensions;

namespace Shinden.API
{
    public class RequestManager
    {
        private Uri _baseUri;
        private Models.ISession _session;
        private readonly CookieContainer _cookies;
        private readonly ILogger<RequestManager> _logger; 
        private readonly Auth _auth;

        public RequestManager(Auth auth, ILogger<RequestManager> logger)
        {
            _auth = auth;
            _session = null;
            _baseUri = null;
            _logger = logger;
            _cookies = new CookieContainer();
        }

        public void WithUserSession(Models.ISession session)
        {
            _session = session;
            AddSessionCookies();
            _logger.Log(LogLevel.Information, "User session has been crated.");
        }

        public async Task<IResponse<T>> QueryAsync<T>(IQuery<T> query) where T : class
        {
            await CheckQuery(query);

            using var handler = new HttpClientHandler() { CookieContainer = _cookies };
            using var client = new HttpClient(handler);
            query.WithToken(_auth.Token);

            client.DefaultRequestHeaders.Add("Accept-Language", "pl");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", $"{_auth.GetUserAgent()}");
            if (_auth.Marmolade != null)
            {
                client.DefaultRequestHeaders.Add(_auth.Marmolade, "marmolada");
            }

            _logger.Log(LogLevel.Information, $"Processing request: [{query.Message.Method}] {query.Uri}");

            if (query.Message.Content != null)
            {
                _logger.Log(LogLevel.Trace,$"Request body: {await query.Message.Content.ReadAsStringAsync()}");
            }

            var response = await client.SendAsync(query.Message).ConfigureAwait(false);

            if (response == null)
            {
                _logger.Log(LogLevel.Critical, "Null response!");
                throw new Exception("Null response!");
            }
                    
            _logger.Log(LogLevel.Information, $"Response code: {(int)response.StatusCode}");

            var final = new ResponseFinal<T>(response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    
            _logger.Log(response.IsSuccessStatusCode ? LogLevel.Trace : LogLevel.Debug, $"Response body: {responseBody}");

            if (!response.IsSuccessStatusCode) return final;
            _logger.Log(LogLevel.Debug, "Parsing response.");

            try
            {
                final.SetBody(query.Parse(responseBody));
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, $"In parsing: {ex}");
            }

            return final;
        }

        private async Task CheckQuery<T>(IQuery<T> query) where T : class
        {
            if (query is LoginUserQuery)
            {
                if (_baseUri == null)
                    _baseUri = new Uri(query.BaseUri);
            }
            else
                await CheckSession();
        }

        private async Task CheckSession()
        {
            if (_session != null)
            {
                if (!_session.IsValid())
                {
                    var nS = await QueryAsync(new LoginUserQuery(_session.GetAuth())).ConfigureAwait(false);
                    if (nS.IsSuccessStatusCode())
                    {
                        _logger.Log(LogLevel.Information, "User session has been renewed.");
                        _session.Renew(nS.Body.ToModel(_session.GetAuth()).Session);
                        AddSessionCookies();
                    }
                }
            }
        }

        private void AddSessionCookies()
        {
            if (_session == null || _baseUri == null || !_session.IsValid()) return;
            _cookies.Add(_baseUri, new Cookie() { Name = "name", Value = _session.Name, Expires = _session.Expires });
            _cookies.Add(_baseUri, new Cookie() { Name = "id", Value = _session.Id, Expires = _session.Expires });
        }
    }
}
