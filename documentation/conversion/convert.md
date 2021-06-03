# Conversion
Contains commands to convert WKT geometries into GeoJson.

Available commands:
- [convert](#gbs-convert)

## gbs convert
Converts WKT geometries into GeoJson.

```
gbs convert [options]
```

### Example

```
$ gbs convert --wkt "MULTILINESTRING ((10 10, 20 20, 10 40),(40 40, 30 30, 40 20, 30 10))"
```

### Options

```
--help          Show help information.
-w|--wkt <wkt>  The WKT geometry to convert.
```
