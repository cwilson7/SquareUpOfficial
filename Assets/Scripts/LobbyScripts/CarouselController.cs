using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class CarouselController : MonoBehaviour
{
    public static CarouselController cc;
    
    public GameObject carouselPrefab;
    public Transform carouselPos;
    public float carouselRadius;
    [SerializeField] public float distanceFromCamera = 7.5f;
    [SerializeField] public float offsetNegativeYDirection = 2;

    public GameObject carousel;
    
    // Start is called before the first frame update
    void Awake()
    {
        InitializeCarousel();
        cc = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeCarousel()
    {
        carousel = Instantiate(carouselPrefab, carouselPos, transform);
        carousel.transform.localScale = new Vector3(carousel.transform.localScale.x, carousel.transform.localScale.y, carousel.transform.localScale.z);
        carousel.transform.position = new Vector3(carousel.transform.position.x, carousel.transform.position.y - offsetNegativeYDirection, carousel.transform.position.z + distanceFromCamera);
    }
}
