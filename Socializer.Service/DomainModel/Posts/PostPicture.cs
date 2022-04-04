using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Posts
{
    public class PostPicture
    {
        public string PictureId { get; set; }
        public string PictureAppPath { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
