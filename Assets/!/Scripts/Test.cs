// Author: DanlangA

using System;
using UnityEngine;

namespace _.Scripts
{
    public class Test : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            print("Long");
        }

        private void OnCollisionStay(Collision other)
        {
            print("Ngu");
        }
    }
}