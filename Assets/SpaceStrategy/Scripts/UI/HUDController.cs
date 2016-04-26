using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SpaceStrategy
{
	public class HUDController : MonoBehaviour
	{

		public Slider healthSlider;
		public Image damageImage;
		public Color flashColour = new Color (1f, 0f, 0f, 0.1f);
		public float flashSpeed = 8f;
		bool damaged = false;

		// Use this for initialization
		void Start ()
		{
			SpaceShipController.shipHit += recievedDamage;
			SpaceShipController.shipReset += resetValues;
			healthSlider.value = 5;
		}

		void resetValues (int health)
		{
			healthSlider.maxValue = health;
			healthSlider.value = health;
		}

		void recievedDamage (int hitPower)
		{
			damaged = true;
			healthSlider.value -= hitPower;
		}

		void Update ()
		{
			// If the player has just been damaged...
			if (damaged) {
				// ... set the colour of the damageImage to the flash colour.
				damageImage.color = flashColour;
			}
		// Otherwise...
		else {
				// ... transition the colour back to clear.
				damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
			}

			// Reset the damaged flag.
			damaged = false;
		}

	}
}