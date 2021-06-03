# Projection

Contains commands to reproject coordinates.

Available commands:
- [to-rds](#gbs-project-to-rds)
- [to-wgs84](#gbs-project-to-wgs84)

## gbs project to-rds
Converts WGS84 coordinates to Rijksdriehoeksstelsel (RDNew) locations. Projects a single coordinate, or batch-processes a file with coordinates. Supported file formats are flat files (`.txt` or `.csv`) and Excel files (version 2007 and higher).

```
gbs project to-rds [options]
```

### Examples

```
# Project a WGS84 coordinate to an RDNew location

$ gbs project to-rds -c "4.15, 53.3" -s ","

# Project a WGS84 coordinate in lat-lon format and include the original coordinate in the result

$ gbs project to-rds -c "53.3, 4.15" -s "," -l true -is

# Project coordinates in lat-lon order from a flat file to RDNew location into an output file
# Skip the first (header) line and specify a semi-colon is used as separator

$ gbs project to-rds -i "C:\input.csv" -o "C:\output.csv" -l true -h true -s ";"

# Project coordinates in lat-lon order from an Excel file to RDNew location into an output file
# Specify the input range as columns A:B and include the original coordinates in the result

$ gbs project to-rds -i "C:\input.xlsx" -o "C:\output.csv" -l true -r "A:B" -is
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
Converts Rijksdriehoekstelsel (RDNew) locations to WGS84 coordinates. Projects a single location, or batch-processes a file with locations. Supported file formats are flat files (`.txt` or `.csv`) and Excel files (version 2007 and higher).

```
gbs project to-wgs84 [options]
```

### Examples

```
# Project an RDNew location in X-Y format into a WGS84 coordinate

$ gbs project to-wgs84 -l "102395, 579549" -s "," -x true

# Project an RDNew location in X-Y format into a coordinate and include the original coordinate in the result
# Round the coordinate to 3 decimals

$ gbs project to-wgs84 -l "102395, 579549" -s "," -x true -is -d 3

# Project locations in X-Y order from a flat file to WGS84 coordinates into an output file
# Skip the first (header) line and specify a semi-colon is used as separator
# Include the original locations in the result and round the coordinates to 2 decimals

$ gbs project to-wgs84 -i "C:\input.txt" -o "C:\output.csv" -x true -h false -s ";" -is -d 2

# Project locations in X-Y order from an Excel file to WGS84 coordinates into an output file
# Specify the input range as columns A:B and include the original coordinates in the result
# Round the coordinates to 6 decimals

$ gbs project to-wgs84 -i "C:\input.xlsx" -o "C:\output.csv" -x true -d 6 -r "A:B" -is
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
-x|--XY-format <true/false>  True to indicate the coordinates are specified in X-Y format, False if they are specified in Y-X format.
```
