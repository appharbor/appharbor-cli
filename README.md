# AppHarbor CLI

The AppHarbor CLI is used to manage AppHarbor applications from the command line.

For more about AppHarbor see [https://appharbor.com/](https://appharbor.com/).

## Setup

The CLI is known to work on Windows. [Download](https://appharbor-cli.s3.amazonaws.com/appharbor-cli-v1.2.exe) and run the installer to get started.

Once installed, you can log in to create and administer AppHarbor applications. Use the `-h` switch to
get more information about each command and any options available.

Example usage: `appharbor deploy -e bin -e obj`

This will deploy the application using the optional "excluded directory name" argument
to avoid uploading compiled binaries. Also note the use of a command "alias".
Aliases are enclosed in paranthesis in the list of available commands.

	$ appharbor login
	Username: friism
	Password: ***************
	Successfully logged in as friism

	$ appharbor
	Usage: appharbor COMMAND [command-options]

	Available commands:

	  app                         #  List your applications
	  app create [name]           #  Create an application ("create")
	  app delete                  #  Delete application
	  app deploy                  #  Deploy current directory ("deploy")
	  app info                    #  Get application details
	  app link [slug]             #  Link directory to an application ("link")
	  app log                     #  Get application log ("log")
	  app open                    #  Open application on appharbor.com ("open")
	  app unlink                  #  Unlink application from directory ("unlink")
	  build                       #  List latest builds
	  config add [key=value]      #  Add configuration variable to application
	  config                      #  List all configuration variables
	  config remove [key1 key2..] #  Remove configuration variable
	  drain add [drain_url]       #  Add a log drain
	  drain                       #  Show log drains and tokens
	  drain remove [drain url]    #  Remove a log drain
	  help                        #  Display help summary
	  hostname add [hostname]     #  Add a hostname
	  hostname                    #  List all associated hostnames
	  hostname remove [hostname]  #  Remove hostname from application
	  user login                  #  Login to AppHarbor ("login")
	  user logout                 #  Logout of AppHarbor ("logout")
	  user                        #  Show currently logged in user

	Common options:
	  -h, --help                  #  Show command help

## Alias

If you use the CLI a lot you might want to create an alias for the appharbor executable. You can do this with a simple batch file called "ah.bat" in the
CLI install directory. The file should have this content:

    appharbor %*

After this you can simply call the commands with for instance `ah config`.

## API

The CLI uses the AppHarbor API, documentation can be found here: [http://support.appharbor.com/kb/api/api-overview](http://support.appharbor.com/kb/api/api-overview)