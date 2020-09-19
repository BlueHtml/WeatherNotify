# WeatherNotify

获取**当天**或**第二天**的天气情况，并发送邮件。
- 早上7点：当天天气
- 晚上8点：明日天气

## 一、添加Secret

这里只说一下要添加的Secret，全部流程请参考[使用Github Actions来完成有道云笔记每日签到](https://blog.guoqianfan.com/2020/08/30/note163-checkin-with-github-actions/)，都是差不多的
- `City_Id`：城市id。城市id有两种：一种是**市级**城市，[戳我查看 json](https://github.com/JHiroGuo/API/blob/master/Meizu_cities.json)；一种是**县级**城市，[戳我查看 json](https://github.com/JHiroGuo/API/blob/master/Meizu_city.json)
- `Email_Conf`: 发件人邮箱配置。详细请自行查找所用邮箱的第三方客户端配置
    ```json
    {
    	"SmtpServer": "smtp.163.com",
    	"SmtpPort": 994,
    	"SmtpUsername": "gg@163.com",
    	"SmtpPassword": "abc",
    
    	"PopServer": "pop.163.com",
    	"PopPort": 995,
    	"PopUsername": "gg@163.com",
    	"PopPassword": "abc"
    }
    ```
- `From_Address`: 发件人
    ```json
    {
    	"Name": "GG",
    	"Address": "gg@163.com"
    }
    ```
- `To_Address`: 收件人
    ```json
    {
    	"Name": "MM",
    	"Address": "mm@139.com"
    }
    ```

## 二、运行结果

![weather-notify-email](https://img.guoqianfan.com/note/2020/09/weather-notify-email.png)
