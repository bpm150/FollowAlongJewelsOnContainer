# Main dockerfile reference:
# https://docs.docker.com/engine/reference/builder/

#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Class #12 3-8-20

# This is the Dockerfile for the ProductCatalogApi
# describes how to make the image (the "iso," if you will)
#Later (on Class #14 3-15-20, will also need to make the project dockerizable)
#
#docker container is an immutable vm
#these child vms running on a host system
#
#config files need to make containerized
#
#instructions written more like shell command
#
#this dockerfile config first, then another config after
#
#
#we already specified that container will be linux vm (when we first created the project)
#
#
#QUESTION:
#Where is specified which linux distro used for the container?
#How does docker decide?
#What is default?
#How to override and specify?
#
#
#The instructions in this file are executed after linux is already installed and
#started up on the vm
#These are instructions for linux vm to go do for us

#Wait, isn't it linux "VMs" (plural?)
#Are not the base and build "things" not seperate images that need to be
#spun up as seperate VMs (at least initially?)
#are base, build, publish and final magic names/reserved words, or are they
#simply variable names?
#seems like the combo of the last FROM/WORKDIR commands determine the
#relative path of the ENTRYPOINT

#
#Instructions for to
#Prepare and deploy my catalog microservice into the docker container/vm
#
#
#We wrote microservice in dotnet core using C#, so need dotnet core sdk

#CLR runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
#library to install
#base is alias, reference on the vm
# dotnet execution engine

WORKDIR /app
#create working directory
#this is where finally built code is deployed to

#this is where to dump all the files and then compile (?)
#recall that C# is JIT compiled, common language, intermediate language
#when deploy to linux, someone needs to JIT compile to run on linux
#dump files here, then asp.net compiler will kick in and compile for linux env

EXPOSE 80
#expose port 80
#tells tomcat to open the port (no IAS on linux)
#running our microservice as a web api
#needed to access this api from the outside world

#recall at first, will have two containers: catalogapi and sql server (stateless design)
#main machine hosts multiple containers
#need to expose port 80 for the db to be able to communicate with the microservice

#now recall that there are two relevant versions of the dotnet sdk
#the first one above was the "slim" version, the CLR runtime, dotnet execution engine


#this second one contains compiler to go and build your code
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
#import this library
#alias as build


WORKDIR /src
#another working directory, for source to be compiled


#this copy command is asking to copy the csproj file to a folder
#on the vm called "ProductCatalogApi/" (source-to-dest)
# THIS WILL NOT WORK.
# FIRST of all, this dockerfile is already inside the ProductCatalogApi/ folder
# there is no ProductCatalogApi/ subfolder from here (on disk, in the dev
# environment on local box)
# ALSO, copying the csproj file is not enough. Need to copy all the code.
# All the code needs to be compiled.
#COPY ["ProductCatalogApi/ProductCatalogApi.csproj", "ProductCatalogApi/"]

#Better:
COPY . .
# copy all files from local folder (folder that this docker file is in)
# to the /src folder on the vm (recall that the src folder is currently
# the working folder on linux
# This is simpler and it works correctly, so win.


# Now recall that we used Nuget packages (Entity Framework)
# This needs to be brought down onto the docker vm
# Must "restore" all nuget packages

#RUN dotnet restore "ProductCatalogApi/ProductCatalogApi.csproj"
# Need to take off the ProductCatalogApi/ the csproj file will be directly
# the /src folder on the vm. There will be no ProductCatalogApi/ subfolder
# don't even need the double quotes now
RUN dotnet restore ProductCatalogApi.csproj
# run, on the linux vm, the dotnet restore command to restore all of the
# nuget packages referenced in the project file


# Now need to build and release my app
# recall already copied all soruce files needed to to the /src directory

#don't need this additional copy command
#COPY . .

#don't have this subfolder. all source is directly in the /src folder
#WORKDIR "/src/ProductCatalogApi"
# also /src is still the working directory

#now build, compile this proj file in release format
# -c specifies configuration Release is smaller footprint (Debug is default for most projects)
RUN dotnet build "ProductCatalogApi.csproj" -c Release -o /app/build
# "dotnet build - Builds a project and all of its dependencies."
# -o specifies the final output folder of the .dll: /app/build

# https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build
#"For executable projects targeting versions earlier than .NET Core 3.0, library dependencies from NuGet are typically NOT copied to the output folder. They're resolved from the NuGet global packages folder at run time. With that in mind, the product of dotnet build isn't ready to be transferred to another machine to run. To create a version of the application that can be deployed, you need to publish it (for example, with the dotnet publish command)."



#new friendly name for what was called build, now called publish
FROM build AS publish
# Same as:
# ?? FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS publish ??

RUN dotnet publish "ProductCatalogApi.csproj" -c Release -o /app/publish
#"dotnet publish - Publishes the application and its dependencies to a folder for deployment to a hosting system."
# "The application's dependencies are copied from the NuGet cache into the output folder."

# https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish
#"The dotnet publish command's output is ready for deployment to a hosting system (for example, a server, PC, Mac, laptop) for execution. It's the only officially supported way to prepare the application for deployment. "


#Now that app is deployed, need to host it

FROM base AS final
# Same as:
# ?? FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS final ??

#reset the working directory from /src to /app
WORKDIR /app

# QUESTION:
# HELP:
#copy from the publish directory down to the app directory
COPY --from=publish /app/publish .
# https://docs.docker.com/engine/reference/builder/#copy

#Confused. The docker docs say:
# "Optionally COPY accepts a flag --from=<name|index> that can be used to set the source location to a previous build stage (created with FROM .. AS <name>) that will be used instead of a build context sent by the user."

# Same as:
# ?? COPY /app/publish mcr.microsoft.com/dotnet/core/sdk:3.1-buster??
# That is, copy from the coresdk image to the /app/publish dir 
# No, I really can't parse what this COPY command does


# Finally, we need to expose it, to host it

# Now you can access the api from loading our dll into memory
# Now your docker container knows how to expose your api

ENTRYPOINT ["dotnet", "ProductCatalogApi.dll"]
# Preferred form:
# ENTRYPOINT ["executable", "param1", "param2"]
# https://docs.docker.com/engine/reference/builder/#entrypoint





# Kal's answer to my question during class:
# Yes, this is like setting the PATH
# Yes, like setting env var and loading these libraries into memory

# Is docker container now ready to go?
# Not yet. :/


# QUESTION:
# EXPERIMENT:
# The build in AS build and the build in dotnet build are totally unrelated
# and AS build could be AS football (as long as other references were replaced)

# Further, the publish in AS publish and the publish in dotnet publish are totally
# unrelated and AS publish could be AS basketball (as long as other references were replaced)


#Sathis' question
#Note there is no IAS on linux
#It's tomcat


#port already opened by tomcat above
# only port 80 open on this box
# also only thing running on this box is our dll
# our dll is all that is reachable through port 80 on this box
# tomcat will now serve up these two dlls on port 80
# (of course, we don't have any controllers yet, so there's nothing there yet)


# at this point, can only test to see if docker is running since have no apis yet
# we are writing first api today, the pic api (probably since it is the flashiest)

# The api will be: given an id, render a picture

# Recall MVC application contains controllers, models and views
# controller is your logic, where you write your code
# multiple api within each controller
# the apis are what are being exposed through port 80

# if pic api is api/pic, then given an id, will render picture for me

# Pause on docker for a bit
# return to code...


# ADD A CONTROLLER
# API Controller - Empty created in the Controllers folder
# To PicController.cs
