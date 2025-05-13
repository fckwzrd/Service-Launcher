using System;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace _123123.Services
{
    public class LocalAuthServer
    {
        private HttpListener _listener;
        private TaskCompletionSource<string> _callbackTask;

        public LocalAuthServer()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://127.0.0.1:8080/callback/");
            Debug.WriteLine("LocalAuthServer initialized with prefix:");
            foreach (var prefix in _listener.Prefixes)
            {
                Debug.WriteLine($"- {prefix}");
            }
        }

        public async Task<string> StartAsync()
        {
            try
            {
                _callbackTask = new TaskCompletionSource<string>();
                _listener.Start();

                Debug.WriteLine("Local auth server started...");

                // Начинаем слушать входящие запросы
                _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);

                // Ждем получения кода авторизации
                return await _callbackTask.Task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting local auth server: {ex.Message}");
                throw;
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            try
            {
                var listener = (HttpListener)result.AsyncState;
                var context = listener.EndGetContext(result);

                Debug.WriteLine($"Received request: {context.Request.Url}");

                // Отправляем ответ пользователю с обработкой кода авторизации
                var response = context.Response;
                var responseString = @"
                    <html>
                    <head>
                        <title>Авторизация</title>
                        <style>
                            body { 
                                background-color: #1a1a1a; 
                                color: white; 
                                font-family: Arial, sans-serif;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                height: 100vh;
                                margin: 0;
                            }
                            .message {
                                text-align: center;
                                padding: 20px;
                                border: 2px solid #e31e24;
                                border-radius: 5px;
                            }
                            h2 { color: #e31e24; }
                        </style>
                    </head>
                    <body>
                        <div class='message'>
                            <h2>Авторизация успешна!</h2>
                            <p>Вы можете закрыть это окно и вернуться в приложение.</p>
                        </div>
                    </body>
                    </html>";

                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/html; charset=UTF-8";
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                // Получаем параметры из URL
                var url = context.Request.Url.ToString();
                Debug.WriteLine($"Processing URL: {url}");

                // Проверяем наличие параметров Discord или Steam
                var discordCode = context.Request.QueryString["code"];
                var steamParams = context.Request.QueryString.AllKeys.Any(k => k?.StartsWith("openid.") == true);

                if (!string.IsNullOrEmpty(discordCode))
                {
                    Debug.WriteLine("Discord authorization code received");
                    _callbackTask.SetResult(discordCode);
                }
                else if (steamParams)
                {
                    Debug.WriteLine("Steam authentication response received");
                    _callbackTask.SetResult(url);
                }
                else if (context.Request.QueryString["error"] != null)
                {
                    var error = context.Request.QueryString["error"];
                    Debug.WriteLine($"Authorization error: {error}");
                    _callbackTask.SetException(new Exception($"Authorization error: {error}"));
                }
                else if (!url.Contains("/callback"))
                {
                    // Если это не callback URL, продолжаем слушать
                    _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in listener callback: {ex.Message}");
                _callbackTask.SetException(ex);
            }
        }

        public void Stop()
        {
            try
            {
                if (_listener?.IsListening == true)
                {
                    _listener.Stop();
                    Debug.WriteLine("Local auth server stopped.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping local auth server: {ex.Message}");
            }
        }
    }
} 