using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon.Common
{
    public interface IStorage
    {
        void SaveLesson(Lesson lesson);
    }
}
