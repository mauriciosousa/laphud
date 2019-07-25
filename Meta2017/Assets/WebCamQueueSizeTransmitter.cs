/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Examples
{
    public class WebCamQueueSizeTransmitter : MonoBehaviour
    {
        #region Public Vars
        public WebCamQueue queue;

        [Header("Function Address")]
        public string Address = "/unityLag";

        [Header("OSC Settings")]
        public OSCTransmitter Transmitter;



        #endregion

        #region Private Vars

        private int frames = 0;
        OSCMessage msg;
        string tmpAddress = "/unityLag";

        #endregion

        #region Private Methods

        OSCMessage setMessage(int value)
        {
            string str = System.DateTime.Now.ToString("o") + ":" + value.ToString();
            msg.Values[0].setString(str); // Using Custom Method, NOT IN extOSC SPEC!
            return msg;
        }

        #endregion

        #region Unity Methods

        protected virtual void Start()
        {
            msg = new OSCMessage(Address);
            msg.AddValue(OSCValue.String(""));
        }

        private void Update()
        {
            var message = setMessage((int)queue.RenderQueueSize);
            Transmitter.Send(message);
        }

        #endregion
    }
}