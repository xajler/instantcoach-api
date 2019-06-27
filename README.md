# InstantCoach app

## .NET Core

Best to install specific version via `dotnet-install` scripts, for unix/linux:

    wget https://dot.net/v1/dotnet-install.sh
    chmod +x dotnet-install
    # or 2.1, also -Version can be applied if particular version needed
    ./dotnet-install.sh -Channel 2.2

.NET Core will be installed in `~/.dotnet` add it to your `$PATH` in bashrc, zshrc, etc

    export PATH=$HOME/.dotnet:$PATH


## Docker

### MSSQL (linux)

Make sure on linux to create `docker` group and add dev user to this group.


    docker pull microsoft/mssql-server-linux
    # Use docker-run.sh or copy this command
    docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Abc$12345' \
               -p 1433:1433 --name sql1 \
               -d microsoft/mssql-server-linux
    # make sure running
    docker ps -a

    # stop mssql instance
    docker stop sql1
    # start mssql instance
    docker start sql1


## Entity Framework


Make sure that Web or class library project have this packages installed:

    dotnet add package Microsoft.EntityFrameworkCore.Design


## .NET SDK 2.0

Note: package `Microsoft.EntityFrameworkCore.Tools.DotNet` is part of .NET SDK 2.1 and above, so it only refers to .NET SDK 2.0.

    dotnet add package Microsoft.EntityFrameworkCore.Tools.DotNet

If class lib is created make sure to change `TargetFramework` from `netstandard2.0` to `netcoreapp2.0`, because EF CLI cannot work with .NET Standard.

Make sure that is `DotNetCliToolReference` and remove it as `PackageReference` if it is set, this is how it should only be:

    <ItemGroup>
        <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.3" />
    </ItemGroup>


Change dir to directory having EF DbContext, either web project or class lib, in this case it is class lib folder and project `core` and going via web project that is startup project `web`:

    cd core
    # -s means startup project 'web' dir in this case
    dotnet ef migrations add Initial -s ../web

    # If there any errors remove migrations, fix errors then run add again
    dotnet ef migrations remove

    # Make sure no errors on creating initial migration
    # Make sure you're running docker mssql instance!!!
    # Then run this command to create database
    dotnet ef database update -s ../web

    # To drop database and start from scratch
    dotnet ef database drop -s ../web

Connection string using `sa` user and password for docker, good for dev and testing, but for production there is `db-create.sh` good MSSQL template for creating in production environment.
