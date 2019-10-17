using DYTA.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    public interface IHasCollider
    {
        RectInt Collider { get; }
    }
}
