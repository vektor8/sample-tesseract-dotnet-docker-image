FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime

RUN apt-get update && apt-get install -y libleptonica-dev
RUN apt-get update && apt-get install -y libtesseract-dev
RUN apt-get update && apt-get install -y libc6-dev
RUN apt-get update && apt-get install -y libjpeg62-turbo-dev
RUN apt-get update && apt-get install -y libgdiplus
RUN apt-get update && apt-get -q -y install tesseract-ocr-eng

WORKDIR /app/x64
RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 libleptonica-1.80.0.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.4.0.1 libtesseract41.so

WORKDIR /app
COPY --from=build /app/published-app /app
#ENTRYPOINT [ "dotnet", "/app/DockerNetExample.dll" ]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet DockerNetExample.dll
