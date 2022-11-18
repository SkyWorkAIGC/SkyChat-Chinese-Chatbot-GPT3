# OpenAPI-Demo

## 简介
### 我们的API几乎可以应用于任何涉及理解或生成自然语言或代码的任务。我们提供了一系列具有不同参数级别的模型，适用于不同的任务，以及微调您自己的自定义模型的能力。这些模型可用于从内容生成到语义搜索和分类的所有内容。更多细节可见[奇点智源官网文档](https://openapi.singularity-ai.com)

# Demo使用教程：
## 模型服务以及Python环境搭建
1. 下载python, 搭建python基础环境
2. [服务下载链接](TBD) 内含semantic_score_clean和user_profile_clean服务、模型以及示例训练数据   
3. 分别解压并分别下载对应服务的依赖库  
     `pip install -r requirements.txt`  
4. [下载对应系统的Cuda并安装](https://developer.nvidia.com/cuda-downloads)  
5. user_profile_clean服务内 下载cuda版torch   
    `pip install torch==1.11.0+cu115 -f https://download.pytorch.org/whl/torch_stable.html`  
6. python server_v3.py 启动本地服务user_profile_clean  
7. python semantic_score_api.py 启动本地服务semantic_score_clean  
***
## Unity版Demo
1.  Release中下载最新的包。
2.  [在openAPI网站](https://openapi.singularity-ai.com/index.html#/login) 或demo中点击注册 注册并认证 获得 api-key 和 api-secret
3.  点击设置 进入设置界面， 对应位置填入key和secret，以及双方姓名，完后点击保存并退出
4.  确认semantic_score_clean和user_profile_clean服务以部署到本地，如果远程部署请在设置中替换对应请求URL
5.  开聊
***
## Python版Demo
### 开发中。。。
***
##### *最低配置要求Nvidia Gfx 1060