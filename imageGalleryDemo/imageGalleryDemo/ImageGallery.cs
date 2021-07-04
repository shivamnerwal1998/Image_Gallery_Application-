using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 
using System.IO;
using C1.Win.C1Tile;

namespace imageGalleryDemo
{
    public partial class ImageGallery : Form
    {
        public ImageGallery()
        {
            InitializeComponent();
        }

        DataFetcher datafetch = new DataFetcher();
        List<ImageItem> imagesList;
        int checkedItems = 0;
        C1.C1Pdf.C1PdfDocument imagePdfDocument = new C1.C1Pdf.C1PdfDocument();


        private async void onSearchClick(object sender, EventArgs e)
        {
            statusStrip1.Visible = true;
            imagesList = await
            datafetch.GetImageData(_searchBox.Text);
             AddTiles(imagesList);
            statusStrip1.Visible = false;

        }
        private void AddTiles(List<ImageItem> imageList)
        {
            _imageTileControl.Groups[0].Tiles.Clear();
            foreach (var imageitem in imageList)
            {
                Tile tile = new Tile();
                tile.HorizontalSize = 2;
                tile.VerticalSize = 2;
                _imageTileControl.Groups[0].Tiles.Add(tile);
                Image img = Image.FromStream(new
               MemoryStream(imageitem.Base64));
                Template tl = new Template();
                ImageElement ie = new ImageElement();
                ie.ImageLayout = ForeImageLayout.Stretch;
                tl.Elements.Add(ie);
                tile.Template = tl;
                tile.Image = img;
            }
        }

        private void onSearchPanelPaint(object sender, PaintEventArgs e)
        {
            Rectangle r = _searchBox.Bounds;
            r.Inflate(3, 3);
            Pen p = new Pen(Color.LightGray);
            e.Graphics.DrawRectangle(p, r);

        }

        private void onExportImageClick(object sender, EventArgs e)
        {
            List<Image> images = new List<Image>();
            foreach (Tile tile in _imageTileControl.Groups[0].Tiles)
            {
                if (tile.Checked)
                {
                    images.Add(tile.Image);
                }
            }
            ConvertToPdf(images);
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = "pdf";
            saveFile.Filter = "PDF files (*.pdf)|*.pdf*";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {

                imagePdfDocument.Save(saveFile.FileName);
            }

            }
        private void ConvertToPdf(List<Image> images)
        {
            RectangleF rect = imagePdfDocument.PageRectangle;
            bool firstPage = true;
            foreach (var selectedimg in images)
            {
                if (!firstPage)
                {
                    imagePdfDocument.NewPage();
                }
                firstPage = false;
                rect.Inflate(-72, -72);
                imagePdfDocument.DrawImage(selectedimg, rect);
            }

        }
        

        private void onExportImagePaint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(_exportImage.Location.X,
_exportImage.Location.Y, _exportImage.Width, _exportImage.Height);
 r.X -= 29;
            r.Y -= 3;
            r.Width--;
            r.Height--;
            Pen p = new Pen(Color.LightGray);
            e.Graphics.DrawRectangle(p, r);
            e.Graphics.DrawLine(p, new Point(0, 43), new
           Point(this.Width, 43));

        }

        private void onTileChecked(object sender, C1.Win.C1Tile.TileEventArgs e)
        {
            checkedItems++;
            _exportImage.Visible = true;

        }

        private void onTileUnchecked(object sender, C1.Win.C1Tile.TileEventArgs e)
        {
            checkedItems--;
            _exportImage.Visible = checkedItems > 0;


        }

        private void onTilePaint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.LightGray);
            e.Graphics.DrawLine(p, 0, 43, 800, 43);

        }

        //adding  All events on ImageGalleryLoad 
        private void ImageGallery_Load(object sender, EventArgs e)
        {
             // event fires when we click on _search button
            _search.Click += new EventHandler(onSearchClick);

            // event fires when we click on exportImage button  
            _exportImage.Click += new EventHandler(onExportImageClick);

            // Tile events 
           // _imageTileControl.Paint += new PaintEventHandler(onTilePaint); 
            

        }
    }
}
