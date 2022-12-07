import random
import sys

import requests
import time
import hashlib
import json

api_key = ''  # 这里需要替换你的APIKey
api_secret = ''  # 这里需要替换你的APISecret
userName = 'tester'
botName = "小爱"
botAge = 18
month = 12
day = 15
weekDay = "天"
season = "冬天"
weather = "晴天"
temperature = 15


url = 'https://openapi.singularity-ai.com/api/v2/generateByKey'
meaing_url = 'http://localhost:8000/semantic_score'
property_draw_url = 'http://localhost:8001/triplets'
conversation = []
timestamp = str(int(time.time()))
prompt = '中国是一个伟大的国家'
model_version = 'benetnasch_common_gpt3'
useMeaning = True
use_property_draw = True


def help_info():
    help = """
usage : python {0}  
""".format(sys.argv[0])
    print(help)


def generate_sent_list():
    l = ['今天什么天气', '今天什么节日', '今天什么节气', '今天星期几', '今年什么年', '今年是哪一年', '你上的哪所大学', '你今年多大', '你会什么', '你体重是多少', '你叫什么名字',
         '你吃早饭了吗', '你在哪', '你在哪个城市生活', '你在干嘛', '你头发什么颜色', '你好', '你学的什么专业', '你是什么发型', '你是什么星座', '你是几月出生的', '你是哪一年出生的',
         '你是机器人吗', '你是男生还是女生', '你是谁', '你有什么爱好', '你有什么特长', '你有宠物吗', '你的工作是什么', '你的生日是什么时候', '你胸围是多少', '你腰围是多少',
         '你臀围是多少', '你自我介绍一下', '你身高是多少', '你骗人', '哈哈哈哈', '我不想上班', '我不漂亮', '我们在哪里', '我叫什么名字', '我喜欢你', '我困了', '我很开心',
         '我很无聊',
         '我很难过', '我肚子疼', '现在什么温度', '现在几点了', '现在是什么季节', '现在是几号', '现在是阴历几号', '给我讲个笑话']
    return l


def check_meaning(ask):
    input_sent = ask
    sent_list = generate_sent_list()
    resp = requests.post(meaning_url, data=json.dumps({'input_sent': input_sent, 'sent_list': sent_list, 'threshold': 0.8}),
                         headers={'content-type': "application/json"}, timeout=5)

    if resp.status_code == 200:
        resp_data = json.loads(resp.content)
        if resp_data['score'] >= 0.8:
            #print(resp_data)
            rst = resp_data['sent'][0]
            #print(rst)
            return lookup_data(rst)


def check_property_draw(conv):
    resp = requests.post(property_draw_url, data=json.dumps({'input': conv}),
                         headers={'content-type': "application/json"}, timeout=5)

    if resp.status_code == 200:
        a = json.loads(resp.content)
        try:
            rst = a['data'][0]
        except:
            return 'null'
        #print(resp.content)
        ret = rst["object"]+ ',' + rst['predicate'] + ',' + rst['subject']

    return ret

def lookup_data(rst):
    f = open("./huashu.csv", 'r', encoding='UTF-8')
    huashu_dic = {}
    count = 0
    for line in f:
        #print(line)
        strs = line.split(',')
        if count >= 1 and strs[7] == 'AI':
            huashu_dic[strs[8]] = strs[1]
        count = count + 1

    possible_answer = [k for k, v in huashu_dic.items() if v == rst]
    if len(possible_answer)<= 0:
        return 'Error: Check huashu doc'
    a = random.choice(possible_answer)
    return a


def fill_property(meaning_answer):
    if "[robot.name]" in meaning_answer:
        meaning_answer = meaning_answer.replace("[robot.name]", botName)
    if "[user.name]" in meaning_answer:
        meaning_answer = meaning_answer.replace("[user.name]", userName)
    if "[robot.age]" in meaning_answer:
        meaning_answer = meaning_answer.replace("[robot.age]", str(botAge))
    return meaning_answer


def talk(ask):

    if useMeaning:
        meaning_answer = check_meaning(ask)
        if meaning_answer is not None:
            conversation.append(userName + ':' + ask)

            meaning_answer = fill_property(meaning_answer)
            print(meaning_answer)
            conversation.append(botName + ':' + meaning_answer)
            talk(input(f"{userName}: "))
            return

    if use_property_draw and len(conversation) >1:
        conv = [conversation[len(conversation)-1], ask]
        property = check_property_draw(conv)
        if property != 'null':
            print("**********提取到屬性可作為記憶使用 " + property)

    p = generate_prompt(ask)
    # print(p)
    sign_content = api_key + api_secret + model_version + p + timestamp
    sign_result = hashlib.md5(sign_content.encode('utf-8')).hexdigest()
    headers = {
        "App-Key": "Bearer " + api_key,
        "timestamp": timestamp,
        "sign": sign_result,
        "Content-Type": "application/json"
    }
    data = {
        "data": {
            "prompt": p,
            "model_version": model_version,
            "param": {
                "generate_length": 500,
                "top_p": 0.4,
                "top_k": 20,
                "repetition_penalty": 1.3,
                "length_penalty": 1.0,
                "min_len": 4,
                "bad_words": [],
                "end_words": ["[EOS]", "\n", "\t"],
                "temperature": 1.0
            }
        }
    }
    try:
        response = requests.post(url, json=data, headers=headers)

        # print(json.loads(response.text))
        reply = json.loads(response.text)['resp_data']['reply']
        answer = str(reply).split(' ')[0]

        print(botName + ': ' +answer)
        conversation.append(botName + ':' + answer)
        talk(input(f"{userName}: "))
    except Exception as e:
        print(e)


def generate_prompt(ask):
    conversation.append(userName + ':' + ask)
    while len(conversation) > 30:
        conversation.remove(0)
    conversation_str = ""
    count = 0
    for x in conversation:
        conversation_str += conversation[count] + '\n'
        count += 1

    pre_prompt = f"时间是{month}月的第{day}天，星期{weekDay}。{season}的一个{weather}, 外头大约{temperature}度。" \
                 f"{userName}是一个男孩子,{botName}是一个活泼的女孩子，{botName}是{userName}的朋友。{userName}现在在北京," \
                 f"{botName}现在在北京。这是一段{userName}和{botName}的对话" + '\n'
    return pre_prompt + conversation_str + botName + ":"


if __name__ == '__main__':
    args = help_info()
    print(f"开始和{botName}对话")
    talk(input(f"{userName}: "))
    #d = ['爱吃什么', '我爱吃牛肉']
    #check_property_draw(d)