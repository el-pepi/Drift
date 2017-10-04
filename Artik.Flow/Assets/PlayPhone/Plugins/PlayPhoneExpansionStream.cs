using UnityEngine;
using System;
using System.Collections;
using System.IO;

namespace PlayPhone
{
	internal class ExpansionStream : Stream 
	{
		private bool disposed;

#if UNITY_ANDROID

		private AndroidJavaObject javaStream;

		internal ExpansionStream(AndroidJavaObject javaStream)
		{
			this.javaStream = javaStream;
		}

#endif

		#region implemented abstract members of Stream

		public override void Flush()
		{
			throw new NotSupportedException();
		}

#if UNITY_ANDROID

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (disposed || javaStream == null)
			{
				throw new InvalidOperationException("Stream disposed");
			}

			return javaStream.Call<int>("read", buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (disposed || javaStream == null)
			{
				throw new InvalidOperationException("Stream disposed");
			}

			if (origin != SeekOrigin.Begin) {
				throw new NotSupportedException("SeekOrigin." + origin + " is not supported. Please use SeekOrigin." + SeekOrigin.Begin);
			}
			return javaStream.Call<long>("skip", offset);
		}

#else

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

#endif

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override bool CanRead 
		{
			get { return true; }
		}

		public override bool CanSeek 
		{
			get { return true; }
		}

		public override bool CanWrite 
		{
			get { return false; }
		}

		public override long Length 
		{
			get { throw new NotSupportedException(); }
		}

		public override long Position 
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}
		#endregion

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			#if UNITY_ANDROID
			javaStream.Call("close");
			javaStream = null;
			#endif

			disposed = true;
		}
	}
}
