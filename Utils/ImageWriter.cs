namespace Raytracer.Utils;

class ImageWriter : IDisposable
{
    private readonly StreamWriter writer;
    private readonly int imageWidth, imageHeight;

    private Color[] image;
    private int imgLen;

    public ImageWriter(string filename, int _imageWidth, int _imageHeight)
    {
        writer = new(filename);
        imageWidth = _imageWidth;
        imageHeight = _imageHeight;

        image = new Color[imageWidth * imageHeight];
        imgLen = 0;

        writer.WriteLine("P3");
        writer.WriteLine("#" + filename);
        writer.WriteLine(imageWidth + " " + imageHeight);
        writer.WriteLine("255");
    }

    private void WriteImage()
    {
        for (int y = 0; y < imageHeight; y++)
        {
            for (int x = 0; x < imageWidth; x++)
            {
                int index = y*imageWidth + x;
                Color c = image[index];

                int r = (int)(c.R * 255);
                int g = (int)(c.G * 255);
                int b = (int)(c.B * 255);

                writer.Write(r + " " + g + " " + b + "  ");
            }

            writer.WriteLine();
        }
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
        WriteImage();

        writer.Dispose();
    }

    public void WritePixel(Color pixelColor)
    {
        image[imgLen++] = pixelColor;

        // if (curRowLength >= imageWidth)
        // {
        //     // Move to next line
        //     writer.WriteLine();
        // }
    }

}