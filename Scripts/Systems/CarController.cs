using UnityEngine;
public class CarController : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private Transform[] points;
    [SerializeField] private float spawnChance;

    private void Start()
    {
        spawnChance = TransitionController.Instance.cityPopulation / 10;
        TickSystem.Instance.On10Tick += SpawnAttempt;
    }
    private void SpawnAttempt(object sender, TickSystem.OnTickEventArgs e)
    {
        int variable = UIController.Instance.hour;
        if (variable > 12) { variable = 24 - variable; }
        if (Random.Range(0, 100) < spawnChance + variable)
        {
            int number = Random.Range(0, 4);
            switch(number)
            {
                case 0:
                    GameObject newCar = Instantiate(car, points[0].position, Quaternion.Euler(0, 0, 0));
                    newCar.GetComponent<Car>().target = points[1];
                    newCar.GetComponent<Car>().StartUp();
                    break;
                case 1:
                    newCar = Instantiate(car, points[2].position, Quaternion.Euler(0, 0, 0));
                    newCar.GetComponent<Car>().target = points[3]; newCar.GetComponent<Car>().StartUp(); break;
                case 2:
                    newCar = Instantiate(car, points[4].position, Quaternion.Euler(0, 0, 0));
                    newCar.GetComponent<Car>().target = points[5]; newCar.GetComponent<Car>().StartUp(); break;
                case 3:
                    newCar = Instantiate(car, points[6].position, Quaternion.Euler(0, 0, 0));
                    newCar.GetComponent<Car>().target = points[7]; newCar.GetComponent<Car>().StartUp(); break;
            }
        }
    }
}
