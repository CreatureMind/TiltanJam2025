using DG.Tweening;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out FileHandler file))
        {
            DeleteFile(file);
        }
    }

    void DeleteFile(FileHandler file)
    {
        if (file.coughtHand != null)
            file.coughtHand.ForceUngrip();

        file.transform.DOKill();
        file.transform.DOScale(Vector3.zero, .5f)
            .SetEase(Ease.InBack, 1.2f);
        file.transform.DOMove(transform.position, .5f)
            .SetEase(Ease.InOutExpo, 2);

        file.GetComponent<Collider2D>().enabled = false;

        Destroy(file.gameObject, .5f);
    }
}
