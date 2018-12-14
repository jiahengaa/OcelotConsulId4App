FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY . /app
ENV ASPNETCORE_ENVIRONMENT="Production" ASPNETCORE_URLS="http://localhost:5000"
# 加速Docker镜像编译之更换软件源
# 或者执行 RUN sed -i 's/http:\/\/archive\.ubuntu\.com\/ubuntu\//http:\/\/mirrors\.aliyun\.com\/ubuntu\//g' /etc/apt/sources.list
#COPY sources.list /etc/apt/
#RUN apt update && apt -y install libgdiplus && cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime

EXPOSE 80
ENTRYPOINT ["dotnet", "ApiTwo.dll"]
