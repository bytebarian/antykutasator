using Antykutasator.Helpers;
using System;
using System.IO;
using System.Net;

namespace Antykutasator.Services
{
    public class SlackClient
    {
        private readonly string _token;
        private readonly string _channelId;

        public SlackClient(ApplicationConfiguration config)
        {
            _token = config.SlackToken;
            _channelId = config.ChannelId;
        }

        public void SlackSendFile(string filePath)
        {
            FileStream str = File.OpenRead(filePath);
            byte[] fBytes = new byte[str.Length];
            str.Read(fBytes, 0, fBytes.Length);
            str.Close();

            var webClient = new WebClient();
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            var fileData = webClient.Encoding.GetString(fBytes);
            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, "Testing.txt", "multipart/form-data", fileData);

            var nfile = webClient.Encoding.GetBytes(package);
            string url = "https://slack.com/api/files.upload?token=" + _token + "&content=" + nfile + "&channels=" + _channelId;

            byte[] resp = webClient.UploadData(url, "POST", nfile);

            var k = System.Text.Encoding.Default.GetString(resp);
        }
    }
}
