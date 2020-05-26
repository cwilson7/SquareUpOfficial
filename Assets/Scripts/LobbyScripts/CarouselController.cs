using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class CarouselController : MonoBehaviour
{
    public static CarouselController cc;
    
    public GameObject carouselPrefab;
    public Transform carouselPos;
    public float carouselRadius, distanceFromCamera, offsetNegativeYDirection;

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
        carousel.transform.localScale = new Vector3(carouselRadius, carousel.transform.localScale.y, carouselRadius);
        carousel.transform.position = new Vector3(carousel.transform.position.x, carousel.transform.position.y - offsetNegativeYDirection, carousel.transform.position.z + distanceFromCamera);
    }
}
