using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class Request : MonoBehaviour
{
    [Header("URL")]
    private string url = "https://openapi.singularity-ai.com/api/v2/generateByKey";
    public string meaningUrl = "http://localhost:8000/semantic_score";
    public string propertyDrawUrl = "http://localhost:8001/triplets";
    
    [Tooltip(" 这是提示栏 ")]
    public string apiKey = "";
    public string apiSecret = "";
    private string model_version = "benetnasch_common_gpt3";
    private string month, day, weekDay, season, weather, temperature, userName, botName;
    public string prompt = "中国是一个伟大的国家";
    public List<string> conversation = new List<string>();

    public bool useHuaShuModel = true;
    public bool usePropertyDrawModel = true;
    private ChatPanelManager cpm;
    private DataProcessor dp;
    
    private void Start()
    {
        dp = GetComponent<DataProcessor>();
    }
    
    [Button]
    public void Try()
    {
        var header = new Hashtable
        {
            {"APP-KEY", "Bearer " + apiKey},
            {"timestamp", GetTimeStamp()},
            {"sign", GetSignResult()}
        };
        var rst = HttpPost(url, GetAIChatJson(), header);
        var ret = JsonUtility.FromJson<Response>(rst);
        print(rst);
        print(ret.resp_data.reply.Split(' ')[0]);
    }

    public void LoadData(string uName, string bName)
    {
        string[] nowtime = System.DateTime.Now.ToString("yyyy:MM:dd").Split(new char[] { ':' });
        month = nowtime[1];
        day = nowtime[2];
        
        string weekstr = DateTime.Now.DayOfWeek.ToString();
        switch (weekstr)
        {
            case "Monday": weekstr = "一"; break;
            case "Tuesday": weekstr = "二"; break;
            case "Wednesday": weekstr = "三"; break;
            case "Thursday": weekstr = "四"; break;
            case "Friday": weekstr = "五"; break;
            case "Saturday": weekstr = "六"; break;
            case "Sunday": weekstr = "日"; break;
        }
        weekDay = weekstr;
        season = "秋天";
        weather = "晴天";
        temperature = "23";
        userName = uName;
        botName = bName;
    }

    private string GeneratePrompt(string ask)
    {
        conversation.Add(userName + ":" + ask);
        var conversationStr = "";
        while (conversation.Count>30)
        {
            conversation.RemoveAt(0);
        }
        for (int i = 0; i < conversation.Count; i++)
        {
            conversationStr += conversation[i] + '\n';
        }
        
        var prePrompt =$"时间是{month}月的第{day}天，星期{weekDay}。{season}的一个{weather}, 外头大约{temperature}度。" +
                       $"{userName}是一个男孩子, {botName}是一个活泼的女孩子，{botName}是{userName}的朋友。{userName}现在在北京," +
                       $" {botName}现在在北京。这是一段{userName}和{botName}的对话" + '\n';
        return prePrompt + conversationStr + botName + ":";
    }
    
    public async void Send2AI(string ask)
    {
        if (useHuaShuModel)
        {
            var meaningAnswer =  await CheckMeaning(ask);
            if (meaningAnswer != null)
            {
                conversation.Add(userName + ":" + ask);
                FindObjectOfType<UIControl>().showToast("触发话术", 2);
                print(meaningAnswer);
                
                //填充属性表属性 或记忆属性
                
                meaningAnswer = FillProperty(meaningAnswer);
                conversation.Add(botName + ":" + meaningAnswer);
                FindObjectOfType<ChatPanelManager>().AddBubble(meaningAnswer, false);
                return;
            }
        }

        if (usePropertyDrawModel && conversation.Count>1)
        {
            var conv = new List<string>()
            {
                conversation[conversation.Count-1],
                ask
            };
            var property = await CheckPropertyDraw(conv);
            if (property != null)
            {
                FindObjectOfType<UIControl>().showToast("提取到屬性可作為記憶使用 " + property, 2);
                print("提取到屬性可作為記憶使用 " + property);
            }
        }
        
        prompt = GeneratePrompt(ask);
        print(prompt);
        var header = new Hashtable
        {
            {"APP-KEY", "Bearer " + apiKey},
            {"timestamp", GetTimeStamp()},
            {"sign", GetSignResult()}
        };
        var rst = await Task.Run(() => HttpPost(url, GetAIChatJson(), header));
        var ret = JsonUtility.FromJson<Response>(rst);
        print(rst);
        var answer = ret.resp_data.reply.Split(' ')[0];
        conversation.Add(botName + ":" + answer);
        FindObjectOfType<ChatPanelManager>().AddBubble(answer, false);
    }

    private string FillProperty(string answer)
    {
        if (answer.Contains("[robot.name]"))
        {
            answer = answer.Replace("[robot.name]", botName);
        }
        if (answer.Contains("[user.name]"))
        {
            answer = answer.Replace("[user.name]", userName);
        }
        if (answer.Contains("[robot.age]"))
        {
            answer = answer.Replace("[robot.age]", 18.ToString());
        }

        return answer;
    }
    
    public string inputSent;

    public List<string> sentList;

    public float meaningThreshold = 0.8f;

    private List<string> convers = new List<string>();

    private async Task<string> CheckPropertyDraw(List<string> conv)
    {
        convers = conv;
        try
        {
            var rst = await Task.Run(() => HttpPost(propertyDrawUrl, GetPropertyDrawJson()));
            var str = JsonConvert.DeserializeObject<JToken>(rst);
            if (str["data"]!.HasValues)
            {
                var d = str["data"][0];
                return d["object"] + "," + d["predicate"] + "," + d["subject"];
            }
            else return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        } 
        
    }

    
    [Button]
    public void CheckPropertyDrawTest(string ask, string answer)
    {
        convers = new List<string>()
        {
            ask,
            answer
        };
        var rst = HttpPost(propertyDrawUrl, GetPropertyDrawJson());
        print(rst);
        var ha = JsonConvert.DeserializeObject<JToken>(rst);
        if (!ha["data"]!.HasValues)
        {
            print("!!!");
        }
        var d = ha["data"][0];
        print(d["object"] + "," + d["predicate"] + "," + d["subject"]);
    }
    
    
    public void PropertyDraw(string ask)
    {
        
    }
    
    // public void Test()
    // {
    //     var a = GenerateSentList();
    //     var test = GetComponent<DataProcessor>();
    //     string rst = "";
    //     foreach (var b in a)
    //     {
    //         if (test.LookUpData(b)!= null)
    //         {
    //             rst += b + ',';
    //         }
    //     }
    //     print(rst);
    // }
    //
    
    
    private string GetTimeStamp()
    {
        TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        var time  = ((int) t.TotalSeconds).ToString();
        return time;
    }
    
    private string GetSignResult()
    {
        var str = apiKey + apiSecret + model_version + prompt + GetTimeStamp();
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        byteArray = md5.ComputeHash(byteArray);

        string hashedValue = "";
        foreach (byte b in byteArray)
        {
            hashedValue += b.ToString("x2");
        }
        return hashedValue;
    }

    

   

    

    #region AIChat
    public class Data
    {
        public data data;
    }

    public class data
    {
        public string prompt;
        public string model_version;
        public Param param;
    }

    public class Param
    {
        public int generate_length = 500;
        public int top_p = 0.4;
        public int top_k = 20;
        public float repetition_penalty = 1.3f;
        public float length_penalty = 1.0f;
        public int min_len = 4;
        public List<string> bad_words = new List<string>();
        public List<string> end_words = new List<string>()
        {
            "[EOS]",
            "\n",
            "\t",
            " "
        };
        public float temperature = 1.0f;

    }
    
    
    [Serializable]
    public class Response
    {
        public int code;
        public string code_msg;
        public string trace_id;
        public Rdata resp_data;
            [Serializable]
            public class Rdata
            {
                public string reply;
                public int status;
            }   
    }

   
    
    #endregion

    [Button]
    private string GetAIChatJson()
    {
        var json = JsonConvert.SerializeObject(new Data()
        {
            data = new data()
            {
                prompt = prompt,
                model_version = model_version,
                param = new Param()
            }
        });
        return json;
    }
    
    #region meaning

    [Serializable]
    public class Meaning
    {
        public string input_sent;
        public List<string> sent_list;
        public float threshold;
    }


    [Serializable]
    public class MeaningResponse
    {
        public List<string> sent;
        public int idx;
        public float score;
    }
    #endregion
    
    private async Task<string> CheckMeaning(string ask)
    {
        inputSent = ask;
        sentList = GenerateSentList();
        try
        {
            var rst = await Task.Run(() => HttpPost(meaningUrl, GetMeaningJson()));
            var mr = JsonUtility.FromJson<MeaningResponse>(rst);
            if (mr.score >= 0.8f)
            {
                print(mr.sent[0]);
                return dp.LookUpData(mr.sent[0]);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        } 
        return null;
    }
    
    [Button]
    public void CheckMeaningTest(string ask)
    {
        inputSent = ask;
        sentList = GenerateSentList();
        
        //var rst = await Task.Run(() => HttpPost(meaningUrl, GetMeaningJson()));
        print(meaningUrl);
        var rst = HttpPost(meaningUrl, GetMeaningJson());
        print(rst);
        var mr = JsonUtility.FromJson<MeaningResponse>(rst);
        if (mr.score >= 0.8f)
        {
            print(mr.sent[0]);
        }
    }
    
    private string GetMeaningJson()
    {
        var json = JsonConvert.SerializeObject(new Meaning()
        {
            input_sent = inputSent,
            sent_list = sentList,
            threshold = meaningThreshold
        });
        return json;
    }
    
    private List<string> GenerateSentList()
    {
        var pre =
            "今天什么天气,今天什么节日,今天什么节气,今天星期几,今年什么年,今年是哪一年,你上的哪所大学,你今年多大,你会什么,你体重是多少,你叫什么名字," +
            "你吃早饭了吗,你在哪,你在哪个城市生活,你在干嘛,你头发什么颜色,你好,你学的什么专业,你是什么发型,你是什么星座,你是几月出生的,你是哪一年出生的," +
            "你是机器人吗,你是男生还是女生,你是谁,你有什么爱好,你有什么特长,你有宠物吗,你的工作是什么,你的生日是什么时候,你胸围是多少,你腰围是多少," +
            "你臀围是多少,你自我介绍一下,你身高是多少,你骗人,哈哈哈哈,我不想上班,我不漂亮,我们在哪里,我叫什么名字,我喜欢你,我困了,我很开心,我很无聊," +
            "我很难过,我肚子疼,现在什么温度,现在几点了,现在是什么季节,现在是几号,现在是阴历几号,给我讲个笑话";
        return pre.Split(',').ToList();
    }
    
    
    #region propertyDraw

    [Serializable]
    public class propertyDraw
    {
        public List<string> input;
    }

    [Serializable]
    public class propertyDrawRes
    {
        public int code;
        public Data data;

        public class Data
        {
            // public string obj;
            // public 
        }
    }
    
    #endregion
    
    private string GetPropertyDrawJson()
    {
        var json = JsonConvert.SerializeObject(new propertyDraw()
        {
            input = convers
        });
        return json;
    }
    
    private static string HttpPost(string url, string param = null, Hashtable headht = null)
        {
            HttpWebRequest request;
            
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            if (headht != null)
            {
                foreach (DictionaryEntry item in headht)
                {
                    request.Headers.Add(item.Key.ToString(), item.Value.ToString());
                }
            }

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }

}
