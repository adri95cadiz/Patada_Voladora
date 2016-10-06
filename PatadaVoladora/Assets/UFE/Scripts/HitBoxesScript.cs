using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class HitBox {
	public BodyPart bodyPart;
   	public HitBoxType type;
	public Transform position;
	public float radius = .5f;
	public Vector2 offSet;
	public float offSetX;
	public float offSetY;
	public CollisionType collisionType;
	[HideInInspector]
	public int state;
	[HideInInspector]
	public float storedRadius;
}

public class HitBoxesScript : MonoBehaviour {
	public bool detect3dHits;
	public HitBox[] hitBoxes;
	
	[HideInInspector]
	public bool isHit;
	[HideInInspector]
	public HurtBox[] activeHurtBoxes;
	[HideInInspector]
	public BlockArea blockableArea;
	
	private ControlsScript controlsScript;
	private MoveSetScript myMoveSetScript;
	
	void Start(){
		controlsScript = transform.parent.gameObject.GetComponent<ControlsScript>();
		myMoveSetScript = GetComponent<MoveSetScript>();
		detect3dHits = UFE.config.detect3D_Hits;
		
		foreach(HitBox hitBox in hitBoxes){
			hitBox.storedRadius = hitBox.radius;
		}
		
		foreach(MoveInfo move in myMoveSetScript.moves){
			foreach(InvincibleBodyParts invBodyPart in move.invincibleBodyParts){
				List<HitBox> invHitBoxes = new List<HitBox>();
				foreach(BodyPart bodyPart in invBodyPart.bodyParts){
					foreach(HitBox hitBox in hitBoxes){
						if (bodyPart == hitBox.bodyPart) {
							invHitBoxes.Add(hitBox);
							break;
						}
					}
				}
				invBodyPart.hitBoxes = invHitBoxes.ToArray();
			}
			
			foreach(HitBox hitBox in hitBoxes){
				if (move.blockableArea.bodyPart == hitBox.bodyPart) {
					move.blockableArea.position = hitBox.position;
					break;
				}
			}
			
			foreach(Hit hit in move.hits){
				foreach(HitBox hitBox in hitBoxes){
					if (hit.pullEnemyIn.characterBodyPart == hitBox.bodyPart) {
						hit.pullEnemyIn.position = hitBox.position;
						break;
					}
				}
				foreach(HurtBox hurtBox in hit.hurtBoxes){
					foreach(HitBox hitBox in hitBoxes){
						if (hurtBox.bodyPart == hitBox.bodyPart) {
							hurtBox.position = hitBox.position;
							break;
						}
					}
				}
			}
			
			foreach(Projectile projectile in move.projectiles){
				foreach(HitBox hitBox in hitBoxes){
					if (projectile.bodyPart == hitBox.bodyPart) {
						projectile.position = hitBox.position;
					}
				}
			}
		}
	}
	
	public Vector3 testCollision(BlockArea blockableArea) {
		if (isHit) return Vector3.zero;
		foreach (HitBox hitBox in hitBoxes) {
			if (hitBox.collisionType == CollisionType.noCollider) continue;
			
			Vector3 blockablePosition = blockableArea.position.position;
			Vector3 hitBoxPosition = hitBox.position.position;
			if (!detect3dHits){
				blockablePosition = new Vector3(blockableArea.position.position.x, blockableArea.position.position.y, -.5f);
				hitBoxPosition = new Vector3(hitBox.position.position.x, hitBox.position.position.y, -.5f);
			}
			blockablePosition += new Vector3(blockableArea.offSet.x * -controlsScript.mirror, blockableArea.offSet.y, 0);
			blockablePosition.x += (blockableArea.radius/2) * -controlsScript.mirror;
			float dist = Vector3.Distance(blockablePosition, hitBoxPosition);
			if (dist <= blockableArea.radius + hitBox.radius) {
				return (blockablePosition + hitBoxPosition)/2;
			}
		}
		return Vector3.zero;
	}
	
	public Vector3 testCollision(HurtBox[] hurtBoxes) {
		if (isHit) return Vector3.zero;
		foreach (HitBox hitBox in hitBoxes) {
			if (hitBox.collisionType == CollisionType.noCollider) continue;
			foreach (HurtBox hurtBox in hurtBoxes) {
				Vector3 hurtBoxPosition = hurtBox.position.position;
				Vector3 hitBoxPosition = hitBox.position.position;
				if (!detect3dHits){
					hurtBoxPosition = new Vector3(hurtBox.position.position.x, hurtBox.position.position.y, -.5f);
					hitBoxPosition = new Vector3(hitBox.position.position.x, hitBox.position.position.y, -.5f);
				}
				float dist = Vector3.Distance(hurtBoxPosition, hitBoxPosition);
				if (dist <= hurtBox.radius + hitBox.radius) {
					hitBox.state = 1;
					isHit = true;
					return (hurtBoxPosition + hitBoxPosition)/2;
				}
			}
		}
		return Vector3.zero;
	}
	
	public bool testCollision(Vector3 projectilePos, float projectileRadius) {
		if (isHit) return false;
		foreach (HitBox hitBox in hitBoxes) {
			if (hitBox.collisionType == CollisionType.noCollider) continue;
			Vector3 hitBoxPosition = hitBox.position.position;
			if (!detect3dHits){
				hitBoxPosition = new Vector3(hitBox.position.position.x, hitBox.position.position.y, 0);
				projectilePos = new Vector3(projectilePos.x, projectilePos.y, 0);
			}
			float dist = Vector3.Distance(hitBoxPosition, projectilePos);
			if (dist <= projectileRadius + hitBox.radius) {
				isHit = true;
				return true;
			}
		}
		return false;
	}
	
	public float testCollision(HitBox[] opHitBoxes) {
		int totalHits = 0;
		foreach (HitBox hitBox in hitBoxes) {
			if (hitBox.collisionType != CollisionType.bodyCollider) continue;
			foreach (HitBox opHitBox in opHitBoxes) {
				if (opHitBox.collisionType != CollisionType.bodyCollider) continue;
				Vector3 opHitBoxPosition = opHitBox.position.position;
				Vector3 hitBoxPosition = hitBox.position.position;
				if (!detect3dHits){
					opHitBoxPosition = new Vector3(opHitBox.position.position.x, opHitBox.position.position.y, 0);
					hitBoxPosition = new Vector3(hitBox.position.position.x, hitBox.position.position.y, 0);
				}
				float dist = Vector3.Distance(opHitBoxPosition, hitBoxPosition);
				if (dist <= opHitBox.radius + hitBox.radius) totalHits ++;
			}
		}
		return totalHits;
	}
	
	public Vector3 getPosition(BodyPart bodyPart){
		foreach(HitBox hitBox in hitBoxes){
			if (bodyPart == hitBox.bodyPart) return hitBox.position.position;
		}
		return Vector3.zero;
	}

	public HitBox[] getHitBoxes(BodyPart[] bodyParts){
		List<HitBox> hitBoxesList = new List<HitBox>();
		foreach(HitBox hitBox in hitBoxes){
			foreach(BodyPart bodyPart in bodyParts){
				if (bodyPart == hitBox.bodyPart) {
					hitBoxesList.Add(hitBox);
					break;
				}
			}
		}

		return hitBoxesList.ToArray();
	}
	
	public void resetHit(){
		if (!isHit) return;
		foreach (HitBox hitBox in hitBoxes) {
			if (hitBox.state == 1) hitBox.state = 0;
		}
		isHit = false;
	}

	public HitBox getStrokeHitBox(){
		if (!isHit) return null;
		foreach (HitBox hitBox in hitBoxes) {
			if (hitBox.state == 1) return hitBox;
		}
		return null;
	}
	
	public void hideHitBoxes(HitBox[] invincibleHitBoxes){
		foreach (HitBox hitBox in invincibleHitBoxes) {
			if (hitBox.radius == -1) continue;
			hitBox.storedRadius = hitBox.radius;
			hitBox.radius = -1;
		}
	}
	
	public void hideHitBoxes(){
		foreach (HitBox hitBox in hitBoxes) {
			if (hitBox.radius == -1) continue;
			hitBox.storedRadius = hitBox.radius;
			hitBox.radius = -1;
		}
	}
	
	public void showHitBoxes(HitBox[] invincibleHitBoxes){
		foreach (HitBox invBox in invincibleHitBoxes) {
			foreach (HitBox hitBox in hitBoxes) {
				if (hitBox.bodyPart == invBox.bodyPart)
					if (hitBox.storedRadius > 0) hitBox.radius = hitBox.storedRadius;
			}
		}
	}
	
	public void showHitBoxes(){
		foreach (HitBox hitBox in hitBoxes) {
			if (hitBox.storedRadius > 0) hitBox.radius = hitBox.storedRadius;
		}
	}
	
	void OnDrawGizmos() {
		// HITBOXES
		foreach (HitBox hitBox in hitBoxes) {
			if (hitBox.state == 1) {
				Gizmos.color = Color.red;
			} else if (isHit){
				Gizmos.color = Color.magenta;
			} else if (hitBox.collisionType == CollisionType.bodyCollider) {	
				Gizmos.color = Color.yellow;
			} else if (hitBox.collisionType == CollisionType.noCollider) {	
				Gizmos.color = Color.white;
			}else{
				Gizmos.color = Color.green;
			}
			Vector3 hitBoxPosition = hitBox.position.position + new Vector3(hitBox.offSetX, hitBox.offSetY, 0);
			if (!detect3dHits) hitBoxPosition.z = -1;
			if (hitBox.radius > 0) Gizmos.DrawWireSphere(hitBoxPosition, hitBox.radius);
		}
		
		
		// HURTBOXES
		if (activeHurtBoxes != null) {
			Gizmos.color = Color.cyan;
			
			foreach (HurtBox hurtBox in activeHurtBoxes) {
				Vector3 hurtBoxPosition;
				if (hurtBox.position == null){
					hurtBoxPosition = getPosition(hurtBox.bodyPart);
					hurtBoxPosition += new Vector3(hurtBox.offSet.x, hurtBox.offSet.y, 0);
				}else{
					hurtBoxPosition = hurtBox.position.position + new Vector3(hurtBox.offSet.x * controlsScript.mirror, hurtBox.offSet.y, 0);
				}
				if (!detect3dHits) hurtBoxPosition.z = -1;
				
				Gizmos.DrawWireSphere(hurtBoxPosition, hurtBox.radius);
			}
		}
		
		
		// BLOCKBOXES
		if (blockableArea != null && blockableArea.radius > 0){
			Gizmos.color = Color.blue;
			
			Vector3 blockableAreaPosition;
			if (blockableArea.position == null){
				blockableAreaPosition = getPosition(blockableArea.bodyPart);
				blockableAreaPosition += new Vector3(blockableArea.offSet.x, blockableArea.offSet.y, 0);
				blockableAreaPosition.x += (blockableArea.radius/2);
			}else{
				blockableAreaPosition = blockableArea.position.position;
				blockableAreaPosition += new Vector3(blockableArea.offSet.x * controlsScript.mirror, blockableArea.offSet.y, 0);
				blockableAreaPosition.x += (blockableArea.radius/2) * controlsScript.mirror;
			}
			if (!detect3dHits) blockableAreaPosition.z = -1;

			Gizmos.DrawWireSphere(blockableAreaPosition, blockableArea.radius);
		}
    }
}
