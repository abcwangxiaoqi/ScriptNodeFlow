using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CodeMind
{
    public abstract class NewBaseCanvas
    {
        public NewBaseCanvas(CodeMindData mindData)
        {
          
        }

        public virtual void Draw()
        {

        }
    }

    public class NewEditorCanvas: NewBaseCanvas
    {
        public NewEditorCanvas(CodeMindData mindData) : base(mindData) { }

        public override void Draw()
        {
        }
    }

    public class NewRuntimeCanvas:NewBaseCanvas
    {
        public NewRuntimeCanvas(CodeMindData mindData) : base(mindData) { }

        public override void Draw()
        {
            throw new System.NotImplementedException();
        }
    }
}
