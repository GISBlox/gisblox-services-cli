# Authentication 
Login, logout, and get information about your authentication state.

Available commands:
- [login](#gbs-login)
- [logout](#gbs-logout)
- [status](#gbs-status)

## gbs login
Authenticate with a GISBlox Services host. 

You must have a personal service key to access GISBlox Services. To generate a service key, create an account in the [GISBlox Account Center](https://account.gisblox.com/) and add a free subscription to the GISBlox Location Services. Once subscribed, click the Location Services tile and copy the service key from the information panel. [More information](http://library.gisblox.com/content/nl-nl/gb1810090).


```
gbs login [options]
```

### Example

```
# authenticate against gisblox.com with a service key.

$ gbs login --with-token MY_SERVICE_KEY

```

### Options
```
--help                            Show help information.
-t|--with-token <service-key>     Authenticate with a GISBlox Services service key.
```

## gbs logout
Log out of a GISBlox Services host. This command removes the authentication configuration.

```
gbs logout [options]
```

### Example

```
$ gbs logout
```

### Options
```
--help   Show help information.
```

## gbs status 
Verifies and displays information about your authentication state.

This command will test your authentication state and reports current subscription details.

```
gbs status [options]
```

### Example

```
$ gbs status
```

### Options
```
--help   Show help information.
```

