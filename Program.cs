using System.Text.Json;
using Util.Mail;

namespace WeatherNotify
{
    class Program
    {
        static async Task Main()
        {
            string cityId = GetEnvValue("CITY_ID");
            IEmailConfiguration emailConf = Deserialize<EmailConfiguration>(GetEnvValue("EMAIL_CONF"));
            EmailAddress FromAddressObj = Deserialize<EmailAddress>(GetEnvValue("FROM_ADDRESS"));
            EmailAddress ToAddressObj = Deserialize<EmailAddress>(GetEnvValue("TO_ADDRESS"));
            IEmailService emailService = new EmailService(emailConf);

            using var client = new HttpClient
            {
                BaseAddress = new Uri("http://aider.meizu.com")
            };
            string result = await client.GetStringAsync($"/app/weather/listWeather?cityIds={cityId}");

            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement root = doc.RootElement;

            if (root.GetProperty("code").GetString() != "200")
            {
                throw new Exception("返回的状态不是200！");
            }

            string dayInfo = "今日";
            DateTime nowTime_CN = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            if (nowTime_CN.Hour > 12)
            {
                nowTime_CN = nowTime_CN.AddDays(1d);
                dayInfo = "明日";
            }

            foreach (JsonElement eleCityWeather in root.GetProperty("value").EnumerateArray())
            {
                var cityName = eleCityWeather.GetProperty("city").GetString();

                //最近几日天气情况
                foreach (var dayWeather in eleCityWeather.GetProperty("weathers").EnumerateArray())
                {
                    DateTime tempDate = dayWeather.GetProperty("date").GetDateTime();
                    if (tempDate.Date == nowTime_CN.Date)
                    {
                        //白天温度，最高温度，摄氏度
                        string dayTemp = dayWeather.GetProperty("temp_day_c").GetString();
                        //夜晚温度，最低温度，摄氏度
                        string nightTemp = dayWeather.GetProperty("temp_night_c").GetString();
                        //wd：风向
                        string wd = dayWeather.GetProperty("wd").GetString();
                        wd = string.IsNullOrWhiteSpace(wd) ? "" : $"，风向：{wd}";
                        //ws：风力大小
                        string ws = dayWeather.GetProperty("ws").GetString();
                        ws = string.IsNullOrWhiteSpace(ws) ? "" : $"，风力：{ws}";
                        //weather：天气情况
                        string weather = dayWeather.GetProperty("weather").GetString();

                        string msg = $"{dayInfo}天气：{weather}，温度：{nightTemp}~{dayTemp}℃{wd}{ws}。";
                        Console.WriteLine(msg);

                        #region 发送邮件

                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.FromAddresses.Add(FromAddressObj);
                        emailMessage.ToAddresses.Add(ToAddressObj);
                        emailMessage.Subject = $"{dayInfo}天气：{weather}";
                        emailMessage.Content = $"{cityName}，{msg}";

                        emailService.Send(emailMessage);

                        #endregion

                        break;
                    }
                }
            }
        }

        static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options);

        static string GetEnvValue(string key) => Environment.GetEnvironmentVariable(key);
    }
}
