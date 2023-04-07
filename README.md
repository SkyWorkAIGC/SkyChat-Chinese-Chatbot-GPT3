# SkyChat
### [简体中文](README.CN.md)
### [English](README.md)

# 模型简介
#### SkyChat是一款基于中文GPT-3 API研发的聊天机器人项目，它除了基本的聊天、对话、你问我答外，还能支持中英文互译、内容续写、对对联、写古诗、生成菜谱、第三人称转述、创建采访问题等多种功能。
![image](https://user-images.githubusercontent.com/120169448/208878752-edde0544-2d1b-4513-b498-d118f3ed4c25.png)

更多细节可见[奇点智源官网文档](https://openapi.singularity-ai.com)

#### 下面是一些示例：

# 效果示例
体验、试用，请访问[奇点智源API试用](https://openapi.singularity-ai.com/index.html#/tryoutIndex)

### 聊天
![image](https://user-images.githubusercontent.com/120169448/208879009-0aefea8b-2183-4b94-b0d0-0351fe3af0d3.png)

### 问答
![image](https://user-images.githubusercontent.com/120169448/208879023-193723a6-caf9-4ff2-ba01-4c5c017326a8.png)

### 生成菜谱
输入：
![image](https://user-images.githubusercontent.com/120169448/208879071-fe0e87fa-c01d-4edb-8b8a-249e6c2e0b72.png)

输出：
![image](https://user-images.githubusercontent.com/120169448/208879104-3fb89264-5526-4f9f-ace6-508f9a606577.png)

### 对对联
![image](https://user-images.githubusercontent.com/120169448/208879500-4a7d644d-9d0d-4dc4-a6a4-0b21b5c891ac.png)

——————————————————————————————
# Demo使用教程：
## 模型服务以及Python环境搭建
## Windows
1. 下载[Anaconda](https://www.anaconda.com/) 勾选添加到环境变量选项  
   <img src="./p/2c75f4cd-d1c1-4e9d-96b2-96c4c246c18b.jpeg" width = "500" height = "330" alt="图片名称" align=center />
2. 下载并解压[semantic_score_clean](http://open-dialogue.singularity-ai.com/open_dialogue/share_model/semantic_score_clean.zip)和[user_profile_clean](http://open-dialogue.singularity-ai.com/open_dialogue/share_model/user_profile_clean.zip)服务、模型以及示例训练数据
3. 打开Anaconda 输入  
   `conda create -n semantic` 创建新环境  
   `conda info --envs` 查看环境   
   `activate semantic` 启动环境   
   `cd C:\你的路径\semantic_score_clean\semantic_score_clean` cd到解压好的semantic_score_clean文件夹  
   `python -m pip uninstall numpy` 删掉初始自带的numpy  
   `python -m pip install -r requirements.txt` 或者 `pip install -r requirements.txt` 安装所需依赖包   
   `python semantic_score_api.py` 运行服务
4. [下载对应系统的Cuda并安装](https://developer.nvidia.com/cuda-downloads)
5. 打开一个新的Anaconda界面 输入  
   `conda create -n userprofile`创建新环境  
   `conda info --envs`查看环境   
   `activate userprofile`启动环境   
   `cd C:\你的路径\user_profile_clean\user_profile_clean`cd  到解压好的user_profile_clean文件夹  
   `python -m pip uninstall numpy`  删掉初始自带的numpy   
   `python -m pip install -r requirements.txt` 或者 `pip install -r requirements.txt`安装所需依赖包   
   `python -m pip install torch==1.11.0+cu115 -f https://download.pytorch.org/whl/torch_stable.html` 下载cuda版torch      
   `python server_v3.py`运行服务

***
## Unity版Demo
1.  Release中下载最新的包。
2.  [在openAPI网站](https://openapi.singularity-ai.com/index.html#/login) 或demo中点击注册 注册并认证 获得 api-key 和 api-secret
3.  点击设置 进入设置界面， 对应位置填入key和secret，以及双方姓名，完后点击保存并退出
4.  确认semantic_score_clean和user_profile_clean服务以部署到本地，如果远程部署请在设置中替换对应请求URL
5.  开聊
***
## Python版Demo
1. [在openAPI网站](https://openapi.singularity-ai.com/index.html#/login) 或demo中点击注册 注册并认证 获得 api-key 和 api-secret
2. 打开 `\你的路径\OpenAPIDemo\PythonDemo\main.py` 在对应位置填入api-key 和 api-secret 并修改需要修改的参数
   <img src="./p/ZmTZD3SgRo.jpg" width = "800" height = "330" alt="图片名称" align=center />
3. 打开Anaconda 输入  
   `conda create -n talk` 创建新环境  
   `conda info --envs` 查看环境   
   `activate talk` 启动环境   
   `cd C:\你的路径\OpenAPIDemo\PythonDemo` 导航到代码所在的文件夹  
   `python -m pip install requests` 下载依赖资源  
   `python main.py` 开始聊天（需确认2个服务以开启）
***
#### *最低配置要求Nvidia GTX 1060

# 加入开发者群
如果您有问题，不妨微信扫码加入开发者群——

![text](https://user-images.githubusercontent.com/120169448/211474572-4e084a69-04d7-4d34-ab93-ef5fc3007b6f.jpg)

感兴趣别忘了star一下~

![cts](https://user-images.githubusercontent.com/120169448/222312125-efea51d6-541f-410a-b7f3-aa5874c735f8.png)
