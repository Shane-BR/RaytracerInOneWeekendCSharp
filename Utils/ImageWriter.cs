namespace Raytracer.Utils;

class ImageWriter : IDisposable
{
    private readonly StreamWriter writer;
    private readonly int imageWidth, imageHeight;

    public ImageWriter(string filename, int _imageWidth, int _imageHeight)
    {
        writer = new(filename);
        imageWidth = _imageWidth;
        imageHeight = _imageHeight;

        writer.WriteLine("P3");
        writer.WriteLine("#" + filename);
        writer.WriteLine(imageWidth + " " + imageHeight);
        writer.WriteLine("255");
    }

    private double LinearToGamma(double linearComponent)
    {
        if(linearComponent > 0)
        {
            return Math.Sqrt(linearComponent);
        }

        return 0;
    }

    public void Dispose()
    {
        writer.Dispose();
    }

    public void WritePixel(int curRowLength, Color pixelColor)
    {
        int r = (int)(255 * LinearToGamma(pixelColor.R));
        int g = (int)(255 * LinearToGamma(pixelColor.G));
        int b = (int)(255 * LinearToGamma(pixelColor.B));

        writer.Write(r + " " + g + " " + b + "  ");

        if (curRowLength >= imageWidth)
        {
            // Move to next line
            writer.WriteLine();
        }
    }

}