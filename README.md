# ImageServer
ImageServer creates resized variants of source images by parsing specially formatted URL requests. The source images can reside on either the same machine as the ImageServer or on a remote machine. The generated image variants are saved as local files on the machine that runs the ImageServer. This makes it convenient to serve static/cached files on subsequent requests, for performance reasons.

Command line scripts can optionally be executed after an image variant has been generated. This is useful for, but not limited to, lossless image optimization. Rate-limiting and logging can be enabled too.

ImageServer is a .NET Standard 2.0 library available as a [Nuget](https://www.nuget.org/packages/ImageServer/) package. The ImageServer.Demo project is a simple ASP.NET Core application that demonstrates the intended usage of the ImageServer library. Make sure to set correct paths in `appsettings.json` before running the demo.



### Anatomy of requests

An URL request should be formatted using this pattern:

`http://example.com/img/[envelope]-[resizemethod]-[quality|compression]-[postprocessing]/image.jpg`

The *envelope* instruction is mandatory. 

* *envelope* should be formatted as *n1*x*n2* where *n1* is the width and *n2* is the height of the envelope. The envelope defines the dimensions of the "box" that the *resizemethod* operates on.

The *resizemethod*, *quality*, *compression* and *postprocessing* instructions are optional. If any of these instructions are not provided in the URL request, a default value, or the value specified in the configuration, will be applied.

* *resizemethod* can be either of the values `contain`, `cover`, `scaledown` or `fill`. These values maps directly to the resize methods of the [ImageFit](https://github.com/1hdn/ImageFit) library. If this instruction is omitted, the `scaledown` value is implied.

* *quality* applies to JPEG images only, and overrides the default JPEG quality. Valid quality values are in the range 0-100. A value of `q75` sets the quality to 75.

* *compression* applies to PNG images only, and overrides the default PNG compression level. Valid compression level values are in the range 1-9. A value of `c6` sets the compression level to 6.

* *postprocessing* instruction accepts the values `pp` or `npp`. The value `pp` enables the post-processing script, if one is configured, regardless of the post-processing script being marked as disabled in the configuration. The value `npp` skips the post-processing script, if one is configured.



### Examples

```
http://example.com/img/250x250/image.jpg
```

```
http://example.com/img/250x250-contain/image.jpg
```

```
http://example.com/img/250x250-cover/image.jpg
```

```
http://example.com/img/250x250-scaledown/image.jpg
```

```
http://example.com/img/250x250-fill/image.jpg
```

```
http://example.com/img/250x250-contain-q50/image.jpg
```

```
http://example.com/img/250x250-contain-c8/image.png
```

```
http://example.com/img/250x250-contain-pp/image.jpg
```

```
http://example.com/img/250x250-contain-npp/image.jpg
```

```
http://example.com/img/250x250-contain-c8-npp/image.png
```
