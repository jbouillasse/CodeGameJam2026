using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class LanceurBoule : MonoBehaviour
{
    [Header("Réglages")]
    public float forceDeTir = 1000f;
    public float distanceMax = 3f;
    public int nombreDeBalles = 3;

    [Header("Liens")]
    public TextMeshProUGUI texteBalles;

    private Chamboule arbitre;
    private Rigidbody2D rb;
    private LineRenderer ligneVisee;
    private bool estEnTrainDeViser = false;
    private bool aTire = false;
    private Vector3 positionDepart;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ligneVisee = GetComponent<LineRenderer>();
        rb.gravityScale = 0;
        positionDepart = transform.position;
        if (ligneVisee != null) ligneVisee.enabled = false;

        arbitre = FindObjectOfType<Chamboule>();

        MettreAJourTexte();
    }

    void Update()
    {
        if (arbitre == null || arbitre.partieFinie || aTire) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 sourisPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (Vector2.Distance(sourisPos, transform.position) < 2.0f)
            {
                estEnTrainDeViser = true;
                if (ligneVisee != null) ligneVisee.enabled = true;
            }
        }

        if (estEnTrainDeViser && Mouse.current.leftButton.isPressed)
        {
            DessinerLaLigne();
        }

        if (estEnTrainDeViser && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Tirer();
        }
    }

    void DessinerLaLigne()
    {
        Vector3 sourisPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        sourisPos.z = 0;
        Vector3 dir = Vector3.ClampMagnitude(transform.position - sourisPos, distanceMax);
        if (ligneVisee != null)
        {
            ligneVisee.SetPosition(0, transform.position);
            ligneVisee.SetPosition(1, transform.position + dir);
        }
    }

    void Tirer()
    {
        estEnTrainDeViser = false;
        aTire = true;
        if (ligneVisee != null) ligneVisee.enabled = false;

        rb.gravityScale = 1;
        Vector3 sourisPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = Vector2.ClampMagnitude(transform.position - sourisPos, distanceMax);
        rb.AddForce(dir * forceDeTir);

        nombreDeBalles--;
        MettreAJourTexte();

        StartCoroutine(GererApresTir());
    }

    IEnumerator GererApresTir()
    {
        yield return new WaitForSeconds(2f);

        if (arbitre.partieFinie) yield break;
        if (nombreDeBalles > 0)
        {
            ResetBalle();
        }
        else
        {
            arbitre.DeclencherFin(false);
            gameObject.SetActive(false);
        }
    }

    void ResetBalle()
    {
        aTire = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0;
        transform.position = positionDepart;
        transform.rotation = Quaternion.identity;
    }

    void MettreAJourTexte()
    {
        if (texteBalles != null) texteBalles.text = "Balles : " + nombreDeBalles;
    }
}