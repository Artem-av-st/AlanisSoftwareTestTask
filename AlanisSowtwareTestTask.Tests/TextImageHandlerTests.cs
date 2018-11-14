using AlanisSoftwareTestTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AlanisSowtwareTestTask.Tests
{
    
    public class TextImageHandlerTests
    {
        TextImageHandler _handler = new TextImageHandler(@"C:\Users\ayakutin\Desktop\Для Артема\4.tif");

        [Fact]
        public void DrawWordsContours()
        {            
            _handler.DrawWordsContours(@"C:\Users\ayakutin\Desktop\Для Артема\2.tif");
        }

        [Fact]
        public void DrawTextRegions()
        {
            _handler.DrawTextRegionsContours(@"C:\Users\ayakutin\Desktop\Для Артема\3.tif");
        }
    }
}
