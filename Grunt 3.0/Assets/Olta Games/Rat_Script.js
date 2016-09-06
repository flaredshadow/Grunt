#pragma strict

function Start () {

}

function Update () {
{
 if(Input.GetKeyDown("1"))
 {
  // Plays the reload animation - stops all other animations
  GetComponent.<Animation>().Play("Rat_Walk", PlayMode.StopAll);
 }
  if(Input.GetKeyDown("2"))
 {
  // Plays the reload animation - stops all other animations
  GetComponent.<Animation>().Play("Rat_Idle1", PlayMode.StopAll);
 }
  if(Input.GetKeyDown("3"))
 {
  // Plays the reload animation - stops all other animations
  GetComponent.<Animation>().Play("Rat_Idle2", PlayMode.StopAll);
 }
  if(Input.GetKeyDown("4"))
 {
  // Plays the reload animation - stops all other animations
  GetComponent.<Animation>().Play("Rat_Death", PlayMode.StopAll);
 }
  if(Input.GetKeyDown("5"))
 {
  // Plays the reload animation - stops all other animations
  GetComponent.<Animation>().Play("Rat_Attack", PlayMode.StopAll);
 }
}
}