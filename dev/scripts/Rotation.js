#pragma strict

//var speed:int;
var x:int;
var y:int;
var z:int;

function Start () {

}

function Update () {
transform.Rotate (x* Time.deltaTime, y * Time.deltaTime, z* Time.deltaTime);
}