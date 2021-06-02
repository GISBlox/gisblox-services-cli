# Projection

Contains commands to reproject coordinates.

Available commands:
- [to-rds](#gbs-project-to-rds)
- [to-wgs84](#gbs-project-to-wgs84)

## gbs project to-rds
Converts WGS84 coordinates to Rijksdriehoeksstelsel (RDNew) locations. Projects a single coordinate, or batch-processes a file with coordinates.

### Usage
```
gbs project to-rds [options]
```

### Options
```
--help                            Show help information.
-c|--coordinate <coordinate>      The coordinate to project to a RDNew location (ignored when an input file is used).
-s|--separator <separator>        Specifies the coordinate separator, e.g. ";" (ignored when the input file is an Excel file).
-is|--include-source              Includes the source coordinate in the result if specified.
-i|--input <input file>           The input file that contains the coordinates to project.
-r|--range <Excel range>          The Excel range that contains the coordinates to project (only if the input file is an Excel file).
-o|--output <output file>         The output file to which to write the result.
-h|--headers <true/false>         True to specify the input file contains headers (ignored when the input file is an Excel file).
-l|--lat-lon-format <true/false>  True to indicate the coordinates are specified in lat-lon format, False if they are specified in lon-lat format.

```

## gbs project to-wgs84
Converts Rijksdriehoekstelsel (RDNew) locations to WGS84 coordinates. Projects a single location, or batch-processes a file with locations.

### Usage
```
gbs project to-wgs84 [options]
```

### Options
```
--help                       Show help information.
-l|--location <location>     The RDNew location to project to a WGS84 coordinate (ignored when an input file is used).
-s|--separator <separator>   Specifies the location separator, e.g. ";" (ignored when the input file is an Excel file).
-d|--decimals <decimals>     Rounds the coordinate(s) to the specified amount of fractional digits.
-is|--include-source         Includes the source location in the result if specified.
-i|--input <input file>      The input file that contains the RDNew locations to project.
-r|--range <Excel range>     The Excel range that contains the coordinates to project (only if the input file is an Excel file).
-o|--output <output file>    The output file to which to write the result.
-h|--headers <true/false>    True to specify the input file contains headers (ignored when the input file is an Excel file).

```
