using System.Collections.Generic;
using UnityEngine;

public class ReceiptPrinterScript : MonoBehaviour
{
    [SerializeField] RectTransform note;
    [SerializeField] GameObject noteGameobject;
    List<string> notesToPrint = new List<string>();
    bool isReceiptWaiting;

    public void printReceipt(string noteToPrint)
    {
        notesToPrint.Add(noteToPrint);
        if (!isReceiptWaiting)
        {
            NextReceipt();
        }
    }

    public void printCountDown()
    {
        Debug.Log("3");
        Debug.Log("2");
        Debug.Log("1");
    }

    public void NextReceipt()
    {
        if (notesToPrint.Count > 0)
        {
            //printReceipt
            Reciept recieptInstance = Instantiate(noteGameobject).GetComponent<Reciept>();
            recieptInstance.receiptPrinter = this;
            recieptInstance.note = note;
            recieptInstance.text = notesToPrint[0];
            notesToPrint.RemoveAt(0);

            isReceiptWaiting = true;
        }
        else
        {
            //update is receipt waiting if called by note and no receipt is left to call
            isReceiptWaiting = false;
        }
    }
}
