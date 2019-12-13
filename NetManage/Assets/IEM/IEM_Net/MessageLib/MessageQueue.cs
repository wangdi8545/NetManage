using Assets.MessageLib;
using System.Collections;

public class MessageQueue
{
	const int MAX_LENGTH = 505;
	private Queue myQueue;

	public MessageQueue ()
	{
		myQueue = new Queue (MAX_LENGTH);
	}

	/// <summary>
	/// Add Message. Return True if Add success.
	/// Return False if Queue is full
	/// </summary>
	public bool AddMessage (Message msg)
	{
		lock (myQueue) {
			if (myQueue.Count < MAX_LENGTH)
				myQueue.Enqueue (msg);
			else {
				// 队列已满，不予进队
				IEM_Log.ins.Log ("[MessageQueue.AddMessage] 队列已满 最多的Command:" + checkQueueWhileQueueFull());
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Get Message from Queue.
	/// Return null if the Queue is empty
	/// or Return jsonObject 
	/// </summary>
	public Message GetMessage ()
	{
		Message temp;
		lock (myQueue) {
			if (myQueue.Count > 0) {
				temp = (Message)myQueue.Dequeue ();
			} else
				return null;
		}
		return temp;
	}

	public int GetCount ()
	{
		return myQueue.Count;
	}

	private string checkQueueWhileQueueFull ()
	{
		ArrayList tempList = new ArrayList ();
		foreach (Message msg in myQueue) {
			tempList.Add (msg.command);
		}
		tempList.Sort ();
		tempList.Add ("##_END_###");
		int len = tempList.Count;
		int nowIndex = 0, maxIndex = 0;
		int maxnum = 1;

		for (int i = 1; i < len; i++) {
			if (tempList [i].Equals (tempList [nowIndex])) {
			} else {
				if (i - nowIndex > maxnum) {
					maxnum = i - nowIndex;
					maxIndex = nowIndex;
				}
				nowIndex = i;
			}

		}
		return tempList [maxIndex].ToString ();
	}

	public void clearMessageQueue()
	{
		lock (myQueue) {
			
			myQueue.Clear ();
		}
	}
    public void removeMessageQueue(Assets.Session.ISession session)
    {
        int count = this.GetCount();
        for(int i = 0; i < count; i++)
        {
            Message temp = this.GetMessage();
            if (temp.session != session) this.AddMessage(temp);
        }
        //lock (myQueue)
        //{
        //    Queue myTempQueue = new Queue(MAX_LENGTH);
        //    while (myQueue.Count > 0)
        //    {
        //        Message temp = (Message)myQueue.Dequeue();
        //        if(temp.session != session)
        //        {
        //            myTempQueue.Enqueue(temp);
        //        }
        //    }
        //    myQueue = myTempQueue;
        //}
    }
}