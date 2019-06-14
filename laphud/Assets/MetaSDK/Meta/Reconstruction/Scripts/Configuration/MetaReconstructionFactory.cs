﻿// Copyright © 2018, Meta Company.  All rights reserved.
// 
// Redistribution and use of this software (the "Software") in binary form, without modification, is 
// permitted provided that the following conditions are met:
// 
// 1.      Redistributions of the unmodified Software in binary form must reproduce the above 
//         copyright notice, this list of conditions and the following disclaimer in the 
//         documentation and/or other materials provided with the distribution.
// 2.      The name of Meta Company (“Meta”) may not be used to endorse or promote products derived 
//         from this Software without specific prior written permission from Meta.
// 3.      LIMITATION TO META PLATFORM: Use of the Software is limited to use on or in connection 
//         with Meta-branded devices or Meta-branded software development kits.  For example, a bona 
//         fide recipient of the Software may incorporate an unmodified binary version of the 
//         Software into an application limited to use on or in connection with a Meta-branded 
//         device, while he or she may not incorporate an unmodified binary version of the Software 
//         into an application designed or offered for use on a non-Meta-branded device.
// 
// For the sake of clarity, the Software may not be redistributed under any circumstances in source 
// code form, or in the form of modified binary code – and nothing in this License shall be construed 
// to permit such redistribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL META COMPANY BE LIABLE FOR ANY DIRECT, 
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
using System;
using UnityEngine;

namespace Meta.Reconstruction
{
    /// <summary>
    /// Creates an <see cref="IMetaReconstruction"/>.
    /// </summary>
    internal class MetaReconstructionFactory
    {
        private IMetaContextInternal _metaContext;
        private Transform _metaReconstructionPrefab;

        /// <summary>
        /// Creates an instance of <see cref="MetaReconstructionFactory"/> class.
        /// </summary>
        /// <param name="metaContext">The Context to acces to the required services.</param>
        /// <param name="environmentScanControllerPrefab">The prefab with the meta reconstruction.</param>
        internal MetaReconstructionFactory(IMetaContextInternal metaContext, Transform metaReconstructionPrefab)
        {
            if (metaContext == null)
            {
                throw new ArgumentNullException("metaContext");
            }

            if (metaReconstructionPrefab == null)
            {
                throw new ArgumentNullException("metaReconstructionPrefab");
            }

            _metaContext = metaContext;
            _metaReconstructionPrefab = metaReconstructionPrefab;
        }

        /// <summary>
        /// Creates an <see cref="IEnvironmentInitializer"/> for the given environment selection result.
        /// </summary>
        /// <param name="environmentInitializaterType">The environment initializer type.</param>
        /// <returns>The environment initializer for the given environment initializer type.</returns>
        public IMetaReconstruction Create(Transform parent)
        {
            Transform metaReconstructionTransform = MonoBehaviour.Instantiate(_metaReconstructionPrefab);
            metaReconstructionTransform.parent = parent;
            metaReconstructionTransform.position = Vector3.zero;

            IMetaReconstruction metaReconstruction = metaReconstructionTransform.GetComponent<IMetaReconstruction>();

            if (metaReconstruction != null)
            {
                if (!_metaContext.ContainsModule<IMetaReconstruction>())
                {
                    _metaContext.Add(metaReconstruction);
                }
                _metaContext.Add<IMeshGenerator>(new MeshGenerator(true, EnvironmentConstants.MaxTriangles));
                _metaContext.Add<IModelFileManipulator>(new OBJFileManipulator());
            }

            return metaReconstruction;
        }
    }
}
