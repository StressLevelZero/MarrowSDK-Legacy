using UnityEngine;
using SLZ.Marrow.Utilities;
namespace SLZ.VRMK
{
    public partial class Avatar : MonoBehaviour
    {
        public virtual void OnDrawGizmosSelected()
        {
            if (animator == null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + transform.rotation * new Vector3(-1f, 1f, 0f), .1f);
                return;
            }

            if (animator.GetBoneTransform(HumanBodyBones.LeftEye) == null && eyeCenterOverride == null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + transform.rotation * new Vector3(-1f, 1f, 0f), .1f);
                return;
            }

            if (requiredBonesOrRefsMissing)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + transform.rotation * new Vector3(-1f, 1f, 0f), .1f);
                return;
            }

            if (!wristLf || !wristRt)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + transform.rotation * new Vector3(-1f, 1f, 0f), .1f);
                return;
            }




            Quaternion worldToAnimator = Quaternion.Inverse(animator.transform.rotation);
            Vector3 eyeInRoot = ((eyeCenterOverride != null) ? eyeCenterOverride.position : animator.GetBoneTransform(HumanBodyBones.LeftEye).position) - animator.transform.position;
            float eyeCenterHeight = (worldToAnimator * eyeInRoot).y;
            Vector3 headTopPos = animator.GetBoneTransform(HumanBodyBones.Head).position;
            headTopPos.y = eyeCenterHeight * (1f + _headTop) + transform.position.y;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(headTopPos + transform.rotation * new Vector3(-.3f, 0f, 0f), headTopPos + transform.rotation * new Vector3(.3f, 0f, 0f));
            Vector3 eyeWorld;
            if (eyeCenterOverride)
            {
                if (animator.GetBoneTransform(HumanBodyBones.LeftEye) != null && animator.GetBoneTransform(HumanBodyBones.RightEye) != null)
                {
                    Vector3 eyeLf = Vector3.Project(eyeCenterOverride.position - animator.GetBoneTransform(HumanBodyBones.LeftEye).position, transform.forward) + animator.GetBoneTransform(HumanBodyBones.LeftEye).position;
                    Vector3 eyeRt = Vector3.Project(eyeCenterOverride.position - animator.GetBoneTransform(HumanBodyBones.RightEye).position, transform.forward) + animator.GetBoneTransform(HumanBodyBones.RightEye).position;
                    Gizmos.DrawSphere(eyeLf + transform.rotation * new Vector3(0f, 0f, eyeOffset * eyeCenterHeight), .005f * eyeCenterHeight);
                    Gizmos.DrawSphere(eyeRt + transform.rotation * new Vector3(0f, 0f, eyeOffset * eyeCenterHeight), .005f * eyeCenterHeight);
                    eyeWorld = Vector3.LerpUnclamped(animator.GetBoneTransform(HumanBodyBones.LeftEye).position, animator.GetBoneTransform(HumanBodyBones.RightEye).position, .5f);
                }
                else
                {
                    Gizmos.DrawSphere(eyeCenterOverride.position + transform.rotation * new Vector3(0f, 0f, eyeOffset * eyeCenterHeight), .005f * eyeCenterHeight);
                    eyeWorld = eyeCenterOverride.position;
                }

            }
            else
            {
                Gizmos.DrawSphere(animator.GetBoneTransform(HumanBodyBones.LeftEye).position + transform.rotation * new Vector3(0f, 0f, eyeOffset * eyeCenterHeight), .005f * eyeCenterHeight);
                Gizmos.DrawSphere(animator.GetBoneTransform(HumanBodyBones.RightEye).position + transform.rotation * new Vector3(0f, 0f, eyeOffset * eyeCenterHeight), .005f * eyeCenterHeight);
                eyeWorld = Vector3.LerpUnclamped(animator.GetBoneTransform(HumanBodyBones.LeftEye).position, animator.GetBoneTransform(HumanBodyBones.RightEye).position, .5f);
            }




            Vector3 neckWorld = animator.GetBoneTransform(HumanBodyBones.Neck).position;
            Vector3 headWorld = animator.GetBoneTransform(HumanBodyBones.Head).position;
            Vector3 shoulderLfWorld = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
            Vector3 shoulderRtWorld = animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            Vector3 sternumWorld = Vector3.LerpUnclamped(shoulderLfWorld, shoulderRtWorld, .5f);
            float sternumEllipseX = (shoulderLfWorld - sternumWorld).magnitude / eyeCenterHeight;

            Vector3 hipLfWorld = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
            Vector3 hipRtWorld = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
            Vector3 hipsWorld = Vector3.LerpUnclamped(hipLfWorld, hipRtWorld, .5f);
            float hipEllipseX = (hipLfWorld - hipsWorld).magnitude / eyeCenterHeight;


            Vector3 waistWorld = Vector3.Lerp(sternumWorld, hipsWorld, _waistY);
            float waistZOffset = (_waistEllipseZ - _waistEllipseNegZ) * .4f;
            waistWorld = worldToAnimator * new Vector3(0f, 0f, waistZOffset * eyeCenterHeight) + waistWorld;
            Vector3 underbustWorld = Vector3.Lerp(sternumWorld, hipsWorld, _underbustY);
            float underbustZOffset = (_chestEllipseZ - _chestEllipseNegZ) * .2f;
            underbustWorld = worldToAnimator * new Vector3(0f, 0f, underbustZOffset * eyeCenterHeight) + underbustWorld;
            Vector3 highHipsWorld = Vector3.Lerp(sternumWorld, hipsWorld, _highHipY);
            float highHipsZOffset = (_highHipsEllipseZ - _highHipsEllipseNegZ) * .4f;
            highHipsWorld = worldToAnimator * new Vector3(0f, 0f, highHipsZOffset * eyeCenterHeight) + highHipsWorld;









            float t1HeightPerc = (neckWorld.y - transform.position.y) / eyeCenterHeight;
            Vector3 sternumOffset = worldToAnimator * (Vector3.LerpUnclamped(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position, animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position, .5f) - animator.GetBoneTransform(HumanBodyBones.Neck).position);
            Vector3 sternumOffsetPerc = sternumOffset / eyeCenterHeight;
            Vector3 hipOffset = worldToAnimator * (Vector3.LerpUnclamped(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position, animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position, .5f) - animator.GetBoneTransform(HumanBodyBones.Hips).position);
            Vector3 hipOffsetPercent = hipOffset / eyeCenterHeight;





            Vector3 fwd = transform.forward;
            Vector3 right = transform.right;
            Vector3 up = Vector3.Cross(fwd, right);

            Vector3 foreheadWorld = (_headTop * eyeCenterHeight * .4f + Vector3.Dot(up, eyeWorld - headWorld)) * up + headWorld;
            Vector3 jawWorld = (_chinY * eyeCenterHeight * -.6f + Vector3.Dot(up, eyeWorld - headWorld)) * up + headWorld;
            Vector3 chinWorld = (_chinY * eyeCenterHeight * -1f + Vector3.Dot(up, eyeWorld - headWorld)) * up + headWorld;

            Vector3 t7Local;
            Vector3 l1Local;
            bool hasUpperChest = animator.GetBoneTransform(HumanBodyBones.UpperChest) != null;
            if (hasUpperChest)
            {
                t7Local = worldToAnimator * (animator.GetBoneTransform(HumanBodyBones.UpperChest).position - animator.GetBoneTransform(HumanBodyBones.Neck).position);
                l1Local = worldToAnimator * (animator.GetBoneTransform(HumanBodyBones.Chest).position - animator.GetBoneTransform(HumanBodyBones.UpperChest).position);
            }
            else
            {
                Vector3 t7l1Halved = worldToAnimator * (animator.GetBoneTransform(HumanBodyBones.Chest).position - animator.GetBoneTransform(HumanBodyBones.Neck).position);
                t7l1Halved *= .5f;
                t7Local = t7l1Halved;
                l1Local = t7l1Halved;
            }
            Vector3 l3Local = worldToAnimator * (animator.GetBoneTransform(HumanBodyBones.Spine).position - animator.GetBoneTransform(HumanBodyBones.Chest).position);
            Vector3 sacrumLocal = worldToAnimator * (animator.GetBoneTransform(HumanBodyBones.Hips).position - animator.GetBoneTransform(HumanBodyBones.Spine).position);







            var spineZ = GenerateSpineCurve(eyeCenterHeight, t7Local, l1Local, l3Local, sacrumLocal, sternumOffsetPerc, hipOffsetPercent, t1HeightPerc);



            var ellipseZ = GenerateTorsoZCurve(sternumOffsetPerc, t1HeightPerc, spineZ);
            var ellipseNegZ = GenerateTorsoNegZCurve(sternumOffsetPerc, t1HeightPerc, spineZ);
            var ellipseX = GenerateTorsoXCurve(sternumEllipseX, hipEllipseX, sternumOffsetPerc, t1HeightPerc);



            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(sternumWorld, Vector3.LerpUnclamped(sternumWorld, underbustWorld, .5f));
            Gizmos.DrawLine(Vector3.LerpUnclamped(sternumWorld, underbustWorld, .5f), underbustWorld);
            Gizmos.DrawLine(underbustWorld, waistWorld);
            Gizmos.DrawLine(waistWorld, highHipsWorld);
            Gizmos.DrawLine(highHipsWorld, Vector3.LerpUnclamped(highHipsWorld, hipsWorld, .5f));
            Gizmos.DrawLine(Vector3.LerpUnclamped(highHipsWorld, hipsWorld, .5f), hipsWorld);


            Gizmos.color = Color.white;
            Vector3 foreheadZ = fwd * _headEllipseZ * eyeCenterHeight;
            Vector3 jawZ = fwd * _jawEllipseZ * eyeCenterHeight;
            Vector3 neckZ = fwd * _neckEllipseZ * eyeCenterHeight;
            Vector3 sternumZ = fwd * _sternumEllipseZ * eyeCenterHeight;
            Vector3 underbustZ = fwd * (_chestEllipseZ - underbustZOffset) * eyeCenterHeight;
            Vector3 waistZ = fwd * (_waistEllipseZ - waistZOffset) * eyeCenterHeight;
            Vector3 highHipsZ = fwd * (_highHipsEllipseZ - highHipsZOffset) * eyeCenterHeight;
            Vector3 hipZ = fwd * _hipsEllipseZ * eyeCenterHeight;

            Gizmos.DrawLine(foreheadWorld + foreheadZ, jawWorld + jawZ);


            Gizmos.DrawLine(headWorld + neckZ, neckWorld + neckZ);

            Gizmos.DrawLine(neckWorld + neckZ, sternumWorld + sternumZ);

            Gizmos.DrawLine(sternumWorld + sternumZ, underbustWorld + underbustZ);

            Gizmos.DrawLine(underbustWorld + underbustZ, waistWorld + waistZ);

            Gizmos.DrawLine(waistWorld + waistZ, highHipsWorld + highHipsZ);

            Gizmos.DrawLine(highHipsWorld + highHipsZ, hipsWorld + hipZ);

            Gizmos.DrawLine(hipsWorld + hipZ, hipsWorld - _crotchBottom * eyeCenterHeight * .3333f * up + (fwd * ellipseZ.Evaluate(1.3333f) * eyeCenterHeight));
            Gizmos.DrawLine(hipsWorld - _crotchBottom * eyeCenterHeight * .3333f * up + (fwd * ellipseZ.Evaluate(1.3333f) * eyeCenterHeight), hipsWorld - _crotchBottom * eyeCenterHeight * .6667f * up + (fwd * ellipseZ.Evaluate(1.6667f) * eyeCenterHeight));
            Gizmos.DrawLine(hipsWorld - _crotchBottom * eyeCenterHeight * .6667f * up + (fwd * ellipseZ.Evaluate(1.6667f) * eyeCenterHeight), hipsWorld - _crotchBottom * eyeCenterHeight * up);



            Gizmos.color = Color.cyan;
            Vector3 foreheadX = right * _headEllipseX * eyeCenterHeight;
            Vector3 jawX = right * _jawEllipseX * eyeCenterHeight;
            Vector3 neckX = right * _neckEllipseX * eyeCenterHeight;
            Vector3 sternumX = right * sternumEllipseX * eyeCenterHeight;
            Vector3 underbustX = right * _chestEllipseX * eyeCenterHeight;
            Vector3 waistX = right * _waistEllipseX * eyeCenterHeight;
            Vector3 highHipsX = right * _highHipsEllipseX * eyeCenterHeight;
            Vector3 hipX = right * _hipsEllipseX * eyeCenterHeight;

            Gizmos.DrawLine(foreheadWorld - foreheadX, jawWorld - jawX);

            Gizmos.DrawLine(headWorld - neckX, neckWorld - neckX);

            Gizmos.DrawLine(shoulderLfWorld, underbustWorld - underbustX);

            Gizmos.DrawLine(underbustWorld - underbustX, waistWorld - waistX);

            Gizmos.DrawLine(waistWorld - waistX, highHipsWorld - highHipsX);

            Gizmos.DrawLine(highHipsWorld - highHipsX, hipsWorld - hipX);


            Gizmos.DrawLine(hipsWorld - hipX, hipsWorld - _crotchBottom * eyeCenterHeight * .3333f * up - (right * ellipseX.Evaluate(1.3333f) * eyeCenterHeight));
            Gizmos.DrawLine(hipsWorld - _crotchBottom * eyeCenterHeight * .3333f * up - (right * ellipseX.Evaluate(1.3333f) * eyeCenterHeight), hipsWorld - _crotchBottom * eyeCenterHeight * .6667f * up - (right * ellipseX.Evaluate(1.6667f) * eyeCenterHeight));
            Gizmos.DrawLine(hipsWorld - _crotchBottom * eyeCenterHeight * .6667f * up - (right * ellipseX.Evaluate(1.6667f) * eyeCenterHeight), hipsWorld - _crotchBottom * eyeCenterHeight * up - (right * ellipseX.Evaluate(2f) * eyeCenterHeight));
            Gizmos.DrawLine(hipsWorld - _crotchBottom * eyeCenterHeight * up - (right * ellipseX.Evaluate(2f) * eyeCenterHeight), hipsWorld - _crotchBottom * eyeCenterHeight * up);


            Gizmos.DrawLine(foreheadWorld + foreheadX, jawWorld + jawX);

            Gizmos.DrawLine(headWorld + neckX, neckWorld + neckX);

            Gizmos.DrawLine(shoulderRtWorld, underbustWorld + underbustX);

            Gizmos.DrawLine(underbustWorld + underbustX, waistWorld + waistX);

            Gizmos.DrawLine(waistWorld + waistX, highHipsWorld + highHipsX);

            Gizmos.DrawLine(highHipsWorld + highHipsX, hipsWorld + hipX);


            Gizmos.DrawLine(hipsWorld + hipX, hipsWorld - _crotchBottom * eyeCenterHeight * .3333f * up + (right * ellipseX.Evaluate(1.3333f) * eyeCenterHeight));
            Gizmos.DrawLine(hipsWorld - _crotchBottom * eyeCenterHeight * .3333f * up + (right * ellipseX.Evaluate(1.3333f) * eyeCenterHeight), hipsWorld - _crotchBottom * eyeCenterHeight * .6667f * up + (right * ellipseX.Evaluate(1.6667f) * eyeCenterHeight));
            Gizmos.DrawLine(hipsWorld - _crotchBottom * eyeCenterHeight * .6667f * up + (right * ellipseX.Evaluate(1.6667f) * eyeCenterHeight), hipsWorld - _crotchBottom * eyeCenterHeight * up + (right * ellipseX.Evaluate(2f) * eyeCenterHeight));
            Gizmos.DrawLine(hipsWorld - _crotchBottom * eyeCenterHeight * up + (right * ellipseX.Evaluate(2f) * eyeCenterHeight), hipsWorld - _crotchBottom * eyeCenterHeight * up);


            Gizmos.color = Color.white;
            Vector3 foreheadNegZ = -fwd * _headEllipseNegZ * eyeCenterHeight;
            Vector3 jawNegZ = -fwd * _jawEllipseNegZ * eyeCenterHeight;
            Vector3 neckNegZ = -fwd * _neckEllipseNegZ * eyeCenterHeight;
            Vector3 sternumNegZ = -fwd * _sternumEllipseNegZ * eyeCenterHeight;
            Vector3 chestNegZ = -fwd * (_chestEllipseNegZ + underbustZOffset) * eyeCenterHeight;
            Vector3 waistNegZ = -fwd * (_waistEllipseNegZ + waistZOffset) * eyeCenterHeight;
            Vector3 highHipsNegZ = -fwd * (_highHipsEllipseNegZ + highHipsZOffset) * eyeCenterHeight;
            Vector3 hipNegZ = -fwd * _hipsEllipseNegZ * eyeCenterHeight;

            Gizmos.DrawLine(foreheadWorld + foreheadNegZ, jawWorld + jawNegZ);

            Gizmos.DrawLine(headWorld + neckNegZ, neckWorld + neckNegZ);

            Gizmos.DrawLine(neckWorld + neckNegZ, sternumWorld + sternumNegZ);

            Gizmos.DrawLine(sternumWorld + sternumNegZ, underbustWorld + chestNegZ);

            Gizmos.DrawLine(underbustWorld + chestNegZ, waistWorld + waistNegZ);

            Gizmos.DrawLine(waistWorld + waistNegZ, highHipsWorld + highHipsNegZ);

            Gizmos.DrawLine(highHipsWorld + highHipsNegZ, hipsWorld + hipNegZ);

            Gizmos.DrawLine(hipsWorld + hipNegZ, hipsWorld - _crotchBottom * eyeCenterHeight * .3333f * up + (-fwd * ellipseNegZ.Evaluate(1.3333f) * eyeCenterHeight));
            Gizmos.DrawLine(hipsWorld - _crotchBottom * eyeCenterHeight * .3333f * up + (-fwd * ellipseNegZ.Evaluate(1.3333f) * eyeCenterHeight), hipsWorld - _crotchBottom * eyeCenterHeight * .6667f * up + (-fwd * ellipseNegZ.Evaluate(1.6667f) * eyeCenterHeight));
            Gizmos.DrawLine(hipsWorld - _crotchBottom * eyeCenterHeight * .6667f * up + (-fwd * ellipseNegZ.Evaluate(1.6667f) * eyeCenterHeight), hipsWorld - _crotchBottom * eyeCenterHeight * up);


            Gizmos.color = new Color(.667f, 1f, 1f, 1f);
            Vector3 foreheadLf30 = foreheadZ * .866f - foreheadX * .5f;
            Vector3 jawLf30 = jawZ * .866f - jawX * .5f;
            Vector3 neckLf30 = neckZ * .866f - neckX * .5f;
            Vector3 sternumLf30 = sternumZ * .866f - sternumX * .5f;
            Vector3 chestLf30 = underbustZ * .866f - underbustX * .5f;
            Vector3 waistLf30 = waistZ * .866f - waistX * .5f;
            Vector3 highHipsLf30 = highHipsZ * .866f - highHipsX * .5f;
            Vector3 hipLf30 = hipZ * .866f - hipX * .5f;
            Gizmos.DrawLine(foreheadWorld + foreheadLf30, jawWorld + jawLf30);
            Gizmos.DrawLine(headWorld + neckLf30, neckWorld + neckLf30);
            Gizmos.DrawLine(neckWorld + neckLf30, sternumWorld + sternumLf30);
            Gizmos.DrawLine(sternumWorld + sternumLf30, underbustWorld + chestLf30);
            Gizmos.DrawLine(underbustWorld + chestLf30, waistWorld + waistLf30);
            Gizmos.DrawLine(waistWorld + waistLf30, highHipsWorld + highHipsLf30);
            Gizmos.DrawLine(highHipsWorld + highHipsLf30, hipsWorld + hipLf30);

            Vector3 foreheadRt30 = foreheadZ * .866f + foreheadX * .5f;
            Vector3 jawRt30 = jawZ * .866f + jawX * .5f;
            Vector3 neckRt30 = neckZ * .866f + neckX * .5f;
            Vector3 sternumRt30 = sternumZ * .866f + sternumX * .5f;
            Vector3 chestRt30 = underbustZ * .866f + underbustX * .5f;
            Vector3 waistRt30 = waistZ * .866f + waistX * .5f;
            Vector3 highHipsRt30 = highHipsZ * .866f + highHipsX * .5f;
            Vector3 hipRt30 = hipZ * .866f + hipX * .5f;
            Gizmos.DrawLine(foreheadWorld + foreheadRt30, jawWorld + jawRt30);
            Gizmos.DrawLine(headWorld + neckRt30, neckWorld + neckRt30);
            Gizmos.DrawLine(neckWorld + neckRt30, sternumWorld + sternumRt30);
            Gizmos.DrawLine(sternumWorld + sternumRt30, underbustWorld + chestRt30);
            Gizmos.DrawLine(underbustWorld + chestRt30, waistWorld + waistRt30);
            Gizmos.DrawLine(waistWorld + waistRt30, highHipsWorld + highHipsRt30);
            Gizmos.DrawLine(highHipsWorld + highHipsRt30, hipsWorld + hipRt30);


            Gizmos.color = new Color(.667f, 1f, 1f, 1f);
            Vector3 foreheadNegLf30 = foreheadNegZ * .866f - foreheadX * .5f;
            Vector3 jawNegLf30 = jawNegZ * .866f - jawX * .5f;
            Vector3 neckNegLf30 = neckNegZ * .866f - neckX * .5f;
            Vector3 sternumNegLf30 = sternumNegZ * .866f - sternumX * .5f;
            Vector3 chestNegLf30 = chestNegZ * .866f - underbustX * .5f;
            Vector3 waistNegLf30 = waistNegZ * .866f - waistX * .5f;
            Vector3 highHipsNegLf30 = highHipsNegZ * .866f - highHipsX * .5f;
            Vector3 hipNegLf30 = hipNegZ * .866f - hipX * .5f;
            Gizmos.DrawLine(foreheadWorld + foreheadNegLf30, jawWorld + jawNegLf30);
            Gizmos.DrawLine(headWorld + neckNegLf30, neckWorld + neckNegLf30);
            Gizmos.DrawLine(neckWorld + neckNegLf30, sternumWorld + sternumNegLf30);
            Gizmos.DrawLine(sternumWorld + sternumNegLf30, underbustWorld + chestNegLf30);
            Gizmos.DrawLine(underbustWorld + chestNegLf30, waistWorld + waistNegLf30);
            Gizmos.DrawLine(waistWorld + waistNegLf30, highHipsWorld + highHipsNegLf30);
            Gizmos.DrawLine(highHipsWorld + highHipsNegLf30, hipsWorld + hipNegLf30);

            Vector3 foreheadNegRt30 = foreheadNegZ * .866f + foreheadX * .5f;
            Vector3 jawNegRt30 = jawNegZ * .866f + jawX * .5f;
            Vector3 neckNegRt30 = neckNegZ * .866f + neckX * .5f;
            Vector3 sternumNegRt30 = sternumNegZ * .866f + sternumX * .5f;
            Vector3 chestNegRt30 = chestNegZ * .866f + underbustX * .5f;
            Vector3 waistNegRt30 = waistNegZ * .866f + waistX * .5f;
            Vector3 highHipsNegRt30 = highHipsNegZ * .866f + highHipsX * .5f;
            Vector3 hipNegRt30 = hipNegZ * .866f + hipX * .5f;
            Gizmos.DrawLine(foreheadWorld + foreheadNegRt30, jawWorld + jawNegRt30);
            Gizmos.DrawLine(headWorld + neckNegRt30, neckWorld + neckNegRt30);
            Gizmos.DrawLine(neckWorld + neckNegRt30, sternumWorld + sternumNegRt30);
            Gizmos.DrawLine(sternumWorld + sternumNegRt30, underbustWorld + chestNegRt30);
            Gizmos.DrawLine(underbustWorld + chestNegRt30, waistWorld + waistNegRt30);
            Gizmos.DrawLine(waistWorld + waistNegRt30, highHipsWorld + highHipsNegRt30);
            Gizmos.DrawLine(highHipsWorld + highHipsNegRt30, hipsWorld + hipNegRt30);


            Gizmos.color = new Color(.333f, 1f, 1f, 1f);
            Vector3 foreheadLf60 = foreheadZ * .5f - foreheadX * .866f;
            Vector3 jawLf60 = jawZ * .5f - jawX * .866f;
            Vector3 neckLf60 = neckZ * .5f - neckX * .866f;
            Vector3 sternumLf60 = sternumZ * .5f - sternumX * .866f;
            Vector3 chestLf60 = underbustZ * .5f - underbustX * .866f;
            Vector3 waistLf60 = waistZ * .5f - waistX * .866f;
            Vector3 highHipsLf60 = highHipsZ * .5f - highHipsX * .866f;
            Vector3 hipLf60 = hipZ * .5f - hipX * .866f;
            Gizmos.DrawLine(foreheadWorld + foreheadLf60, jawWorld + jawLf60);
            Gizmos.DrawLine(headWorld + neckLf60, neckWorld + neckLf60);
            Gizmos.DrawLine(neckWorld + neckLf60, sternumWorld + sternumLf60);
            Gizmos.DrawLine(sternumWorld + sternumLf60, underbustWorld + chestLf60);
            Gizmos.DrawLine(underbustWorld + chestLf60, waistWorld + waistLf60);
            Gizmos.DrawLine(waistWorld + waistLf60, highHipsWorld + highHipsLf60);
            Gizmos.DrawLine(highHipsWorld + highHipsLf60, hipsWorld + hipLf60);

            Vector3 foreheadRt60 = foreheadZ * .5f + foreheadX * .866f;
            Vector3 jawRt60 = jawZ * .5f + jawX * .866f;
            Vector3 neckRt60 = neckZ * .5f + neckX * .866f;
            Vector3 sternumRt60 = sternumZ * .5f + sternumX * .866f;
            Vector3 chestRt60 = underbustZ * .5f + underbustX * .866f;
            Vector3 waistRt60 = waistZ * .5f + waistX * .866f;
            Vector3 highHipsRt60 = highHipsZ * .5f + highHipsX * .866f;
            Vector3 hipRt60 = hipZ * .5f + hipX * .866f;
            Gizmos.DrawLine(foreheadWorld + foreheadRt60, jawWorld + jawRt60);
            Gizmos.DrawLine(headWorld + neckRt60, neckWorld + neckRt60);
            Gizmos.DrawLine(neckWorld + neckRt60, sternumWorld + sternumRt60);
            Gizmos.DrawLine(sternumWorld + sternumRt60, underbustWorld + chestRt60);
            Gizmos.DrawLine(underbustWorld + chestRt60, waistWorld + waistRt60);
            Gizmos.DrawLine(waistWorld + waistRt60, highHipsWorld + highHipsRt60);
            Gizmos.DrawLine(highHipsWorld + highHipsRt60, hipsWorld + hipRt60);


            Gizmos.color = new Color(.333f, 1f, 1f, 1f);
            Vector3 foreheadNegLf60 = foreheadNegZ * .5f - foreheadX * .866f;
            Vector3 jawNegLf60 = jawNegZ * .5f - jawX * .866f;
            Vector3 neckNegLf60 = neckNegZ * .5f - neckX * .866f;
            Vector3 sternumNegLf60 = sternumNegZ * .5f - sternumX * .866f;
            Vector3 chestNegLf60 = chestNegZ * .5f - underbustX * .866f;
            Vector3 waistNegLf60 = waistNegZ * .5f - waistX * .866f;
            Vector3 highHipsNegLf60 = highHipsNegZ * .5f - highHipsX * .866f;
            Vector3 hipNegLf60 = hipNegZ * .5f - hipX * .866f;
            Gizmos.DrawLine(foreheadWorld + foreheadNegLf60, jawWorld + jawNegLf60);
            Gizmos.DrawLine(headWorld + neckNegLf60, neckWorld + neckNegLf60);
            Gizmos.DrawLine(neckWorld + neckNegLf60, sternumWorld + sternumNegLf60);
            Gizmos.DrawLine(sternumWorld + sternumNegLf60, underbustWorld + chestNegLf60);
            Gizmos.DrawLine(underbustWorld + chestNegLf60, waistWorld + waistNegLf60);
            Gizmos.DrawLine(waistWorld + waistNegLf60, highHipsWorld + highHipsNegLf60);
            Gizmos.DrawLine(highHipsWorld + highHipsNegLf60, hipsWorld + hipNegLf60);

            Vector3 foreheadNegRt60 = foreheadNegZ * .5f + foreheadX * .866f;
            Vector3 jawNegRt60 = jawNegZ * .5f + jawX * .866f;
            Vector3 neckNegRt60 = neckNegZ * .5f + neckX * .866f;
            Vector3 sternumNegRt60 = sternumNegZ * .5f + sternumX * .866f;
            Vector3 chestNegRt60 = chestNegZ * .5f + underbustX * .866f;
            Vector3 waistNegRt60 = waistNegZ * .5f + waistX * .866f;
            Vector3 highHipsNegRt60 = highHipsNegZ * .5f + highHipsX * .866f;
            Vector3 hipNegRt60 = hipNegZ * .5f + hipX * .866f;
            Gizmos.DrawLine(foreheadWorld + foreheadNegRt60, jawWorld + jawNegRt60);
            Gizmos.DrawLine(headWorld + neckNegRt60, neckWorld + neckNegRt60);
            Gizmos.DrawLine(neckWorld + neckNegRt60, sternumWorld + sternumNegRt60);
            Gizmos.DrawLine(sternumWorld + sternumNegRt60, underbustWorld + chestNegRt60);
            Gizmos.DrawLine(underbustWorld + chestNegRt60, waistWorld + waistNegRt60);
            Gizmos.DrawLine(waistWorld + waistNegRt60, highHipsWorld + highHipsNegRt60);
            Gizmos.DrawLine(highHipsWorld + highHipsNegRt60, hipsWorld + hipNegRt60);


            Gizmos.color = Color.white;
            Gizmos.DrawLine(foreheadWorld + foreheadLf30, foreheadWorld + foreheadZ);
            Gizmos.DrawLine(foreheadWorld + foreheadZ, foreheadWorld + foreheadRt30);
            Gizmos.DrawLine(jawWorld + jawLf30, jawWorld + jawZ);
            Gizmos.DrawLine(jawWorld + jawZ, jawWorld + jawRt30);
            Gizmos.DrawLine(neckWorld + neckLf30, neckWorld + neckZ);
            Gizmos.DrawLine(neckWorld + neckZ, neckWorld + neckRt30);
            Gizmos.DrawLine(sternumWorld + sternumLf30, sternumWorld + sternumZ);
            Gizmos.DrawLine(sternumWorld + sternumZ, sternumWorld + sternumRt30);
            Gizmos.DrawLine(underbustWorld + chestLf30, underbustWorld + underbustZ);
            Gizmos.DrawLine(underbustWorld + underbustZ, underbustWorld + chestRt30);
            Gizmos.DrawLine(waistWorld + waistLf30, waistWorld + waistZ);
            Gizmos.DrawLine(waistWorld + waistZ, waistWorld + waistRt30);
            Gizmos.DrawLine(highHipsWorld + highHipsLf30, highHipsWorld + highHipsZ);
            Gizmos.DrawLine(highHipsWorld + highHipsZ, highHipsWorld + highHipsRt30);
            Gizmos.DrawLine(hipsWorld + hipLf30, hipsWorld + hipZ);
            Gizmos.DrawLine(hipsWorld + hipZ, hipsWorld + hipRt30);

            Gizmos.DrawLine(foreheadWorld + foreheadNegLf30, foreheadWorld + foreheadNegZ);
            Gizmos.DrawLine(foreheadWorld + foreheadNegZ, foreheadWorld + foreheadNegRt30);
            Gizmos.DrawLine(jawWorld + jawNegLf30, jawWorld + jawNegZ);
            Gizmos.DrawLine(jawWorld + jawNegZ, jawWorld + jawNegRt30);
            Gizmos.DrawLine(neckWorld + neckNegLf30, neckWorld + neckNegZ);
            Gizmos.DrawLine(neckWorld + neckNegZ, neckWorld + neckNegRt30);
            Gizmos.DrawLine(sternumWorld + sternumNegLf30, sternumWorld + sternumNegZ);
            Gizmos.DrawLine(sternumWorld + sternumNegZ, sternumWorld + sternumNegRt30);
            Gizmos.DrawLine(underbustWorld + chestNegLf30, underbustWorld + chestNegZ);
            Gizmos.DrawLine(underbustWorld + chestNegZ, underbustWorld + chestNegRt30);
            Gizmos.DrawLine(waistWorld + waistNegLf30, waistWorld + waistNegZ);
            Gizmos.DrawLine(waistWorld + waistNegZ, waistWorld + waistNegRt30);
            Gizmos.DrawLine(highHipsWorld + highHipsNegLf30, highHipsWorld + highHipsNegZ);
            Gizmos.DrawLine(highHipsWorld + highHipsNegZ, highHipsWorld + highHipsNegRt30);
            Gizmos.DrawLine(hipsWorld + hipNegLf30, hipsWorld + hipNegZ);
            Gizmos.DrawLine(hipsWorld + hipNegZ, hipsWorld + hipNegRt30);

            Gizmos.color = new Color(.667f, 1f, 1f, 1f);
            Gizmos.DrawLine(foreheadWorld + foreheadLf60, foreheadWorld + foreheadLf30);
            Gizmos.DrawLine(foreheadWorld + foreheadRt30, foreheadWorld + foreheadRt60);
            Gizmos.DrawLine(jawWorld + jawLf60, jawWorld + jawLf30);
            Gizmos.DrawLine(jawWorld + jawRt30, jawWorld + jawRt60);
            Gizmos.DrawLine(neckWorld + neckLf60, neckWorld + neckLf30);
            Gizmos.DrawLine(neckWorld + neckRt30, neckWorld + neckRt60);
            Gizmos.DrawLine(sternumWorld + sternumLf60, sternumWorld + sternumLf30);
            Gizmos.DrawLine(sternumWorld + sternumRt30, sternumWorld + sternumRt60);
            Gizmos.DrawLine(underbustWorld + chestLf60, underbustWorld + chestLf30);
            Gizmos.DrawLine(underbustWorld + chestRt30, underbustWorld + chestRt60);
            Gizmos.DrawLine(waistWorld + waistLf60, waistWorld + waistLf30);
            Gizmos.DrawLine(waistWorld + waistRt30, waistWorld + waistRt60);
            Gizmos.DrawLine(highHipsWorld + highHipsLf60, highHipsWorld + highHipsLf30);
            Gizmos.DrawLine(highHipsWorld + highHipsRt30, highHipsWorld + highHipsRt60);
            Gizmos.DrawLine(hipsWorld + hipLf60, hipsWorld + hipLf30);
            Gizmos.DrawLine(hipsWorld + hipRt30, hipsWorld + hipRt60);

            Gizmos.DrawLine(foreheadWorld + foreheadNegLf60, foreheadWorld + foreheadNegLf30);
            Gizmos.DrawLine(foreheadWorld + foreheadNegRt30, foreheadWorld + foreheadNegRt60);
            Gizmos.DrawLine(jawWorld + jawNegLf60, jawWorld + jawNegLf30);
            Gizmos.DrawLine(jawWorld + jawNegRt30, jawWorld + jawNegRt60);
            Gizmos.DrawLine(neckWorld + neckNegLf60, neckWorld + neckNegLf30);
            Gizmos.DrawLine(neckWorld + neckNegRt30, neckWorld + neckNegRt60);
            Gizmos.DrawLine(sternumWorld + sternumNegLf60, sternumWorld + sternumNegLf30);
            Gizmos.DrawLine(sternumWorld + sternumNegRt30, sternumWorld + sternumNegRt60);
            Gizmos.DrawLine(underbustWorld + chestNegLf60, underbustWorld + chestNegLf30);
            Gizmos.DrawLine(underbustWorld + chestNegRt30, underbustWorld + chestNegRt60);
            Gizmos.DrawLine(waistWorld + waistNegLf60, waistWorld + waistNegLf30);
            Gizmos.DrawLine(waistWorld + waistNegRt30, waistWorld + waistNegRt60);
            Gizmos.DrawLine(highHipsWorld + highHipsNegLf60, highHipsWorld + highHipsNegLf30);
            Gizmos.DrawLine(highHipsWorld + highHipsNegRt30, highHipsWorld + highHipsNegRt60);
            Gizmos.DrawLine(hipsWorld + hipNegLf60, hipsWorld + hipNegLf30);
            Gizmos.DrawLine(hipsWorld + hipNegRt30, hipsWorld + hipNegRt60);

            Gizmos.color = new Color(.333f, 1f, 1f, 1f);
            Gizmos.DrawLine(foreheadWorld - foreheadX, foreheadWorld + foreheadLf60);
            Gizmos.DrawLine(foreheadWorld + foreheadRt60, foreheadWorld + foreheadX);
            Gizmos.DrawLine(jawWorld - jawX, jawWorld + jawLf60);
            Gizmos.DrawLine(jawWorld + jawRt60, jawWorld + jawX);
            Gizmos.DrawLine(neckWorld - neckX, neckWorld + neckLf60);
            Gizmos.DrawLine(neckWorld + neckRt60, neckWorld + neckX);
            Gizmos.DrawLine(sternumWorld - sternumX, sternumWorld + sternumLf60);
            Gizmos.DrawLine(sternumWorld + sternumRt60, sternumWorld + sternumX);
            Gizmos.DrawLine(underbustWorld - underbustX, underbustWorld + chestLf60);
            Gizmos.DrawLine(underbustWorld + chestRt60, underbustWorld + underbustX);
            Gizmos.DrawLine(waistWorld - waistX, waistWorld + waistLf60);
            Gizmos.DrawLine(waistWorld + waistRt60, waistWorld + waistX);
            Gizmos.DrawLine(highHipsWorld - highHipsX, highHipsWorld + highHipsLf60);
            Gizmos.DrawLine(highHipsWorld + highHipsRt60, highHipsWorld + highHipsX);
            Gizmos.DrawLine(hipsWorld - hipX, hipsWorld + hipLf60);
            Gizmos.DrawLine(hipsWorld + hipRt60, hipsWorld + hipX);

            Gizmos.DrawLine(foreheadWorld - foreheadX, foreheadWorld + foreheadNegLf60);
            Gizmos.DrawLine(foreheadWorld + foreheadNegRt60, foreheadWorld + foreheadX);
            Gizmos.DrawLine(jawWorld - jawX, jawWorld + jawNegLf60);
            Gizmos.DrawLine(jawWorld + jawNegRt60, jawWorld + jawX);
            Gizmos.DrawLine(neckWorld - neckX, neckWorld + neckNegLf60);
            Gizmos.DrawLine(neckWorld + neckNegRt60, neckWorld + neckX);
            Gizmos.DrawLine(sternumWorld - sternumX, sternumWorld + sternumNegLf60);
            Gizmos.DrawLine(sternumWorld + sternumNegRt60, sternumWorld + sternumX);
            Gizmos.DrawLine(underbustWorld - underbustX, underbustWorld + chestNegLf60);
            Gizmos.DrawLine(underbustWorld + chestNegRt60, underbustWorld + underbustX);
            Gizmos.DrawLine(waistWorld - waistX, waistWorld + waistNegLf60);
            Gizmos.DrawLine(waistWorld + waistNegRt60, waistWorld + waistX);
            Gizmos.DrawLine(highHipsWorld - highHipsX, highHipsWorld + highHipsNegLf60);
            Gizmos.DrawLine(highHipsWorld + highHipsNegRt60, highHipsWorld + highHipsX);
            Gizmos.DrawLine(hipsWorld - hipX, hipsWorld + hipNegLf60);
            Gizmos.DrawLine(hipsWorld + hipNegRt60, hipsWorld + hipX);

















































































































































































            DrawBulge(bulgeBreast, 0f, UnderbustY, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight);
            DrawBulge(bulgeBreast, 0f, UnderbustY, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, true);

            DrawBulge(bulgeUpperBack, 0f, UnderbustY, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, false, true);
            DrawBulge(bulgeUpperBack, 0f, UnderbustY, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, true, true);

            DrawBulge(bulgeAbdomen, UnderbustY, HighHipsY, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, false);
            DrawBulge(bulgeAbdomen, UnderbustY, HighHipsY, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, true);

            DrawBulge(bulgeLowerBack, UnderbustY, HighHipsY, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, false, true);
            DrawBulge(bulgeLowerBack, UnderbustY, HighHipsY, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, true, true);

            DrawBulge(bulgeGroin, HighHipsY, 2f, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, false);
            DrawBulge(bulgeGroin, HighHipsY, 2f, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, true);

            DrawBulge(bulgeButt, HighHipsY, 2f, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, false, true);
            DrawBulge(bulgeButt, HighHipsY, 2f, sternumWorld, hipsWorld, fwd, right, spineZ, ellipseX, ellipseZ, ellipseNegZ, eyeCenterHeight, true, true);










            Vector3 humerousLf = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position - animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
            float humerousLfLength = humerousLf.magnitude / eyeCenterHeight;
            DrawLimb(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm), animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), wristLf,
                _upperarmEllipse, _elbowEllipse, _forearmEllipse, _wristEllipse, Mathf.LerpUnclamped(.03f / humerousLfLength, 1f, .1f), 1f, .18f, 1f, eyeCenterHeight, true);


            Vector3 humerousRt = animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position - animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            float humerousRtLength = humerousRt.magnitude / eyeCenterHeight;
            DrawLimb(animator.GetBoneTransform(HumanBodyBones.RightUpperArm), animator.GetBoneTransform(HumanBodyBones.RightLowerArm), wristRt,
                _upperarmEllipse, _elbowEllipse, _forearmEllipse, _wristEllipse, Mathf.LerpUnclamped(.03f / humerousRtLength, 1f, .1f), 1f, .18f, 1f, eyeCenterHeight);


            Vector3 femurLf = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position - animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
            float femurLfLength = femurLf.magnitude / eyeCenterHeight;
            DrawLimb(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg), animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg), animator.GetBoneTransform(HumanBodyBones.LeftFoot),
                _thighUpperEllipse, _kneeEllipse, _calfEllipse, _ankleEllipse, Mathf.LerpUnclamped(_crotchBottom / femurLfLength, 1f, .1f), 1f, .3f, 1f, eyeCenterHeight, true);


            Vector3 femurRt = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position - animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
            float femurRtLength = femurRt.magnitude / eyeCenterHeight;
            DrawLimb(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg), animator.GetBoneTransform(HumanBodyBones.RightLowerLeg), animator.GetBoneTransform(HumanBodyBones.RightFoot),
                _thighUpperEllipse, _kneeEllipse, _calfEllipse, _ankleEllipse, Mathf.LerpUnclamped(_crotchBottom / femurRtLength, 1f, .1f), 1f, .3f, 1f, eyeCenterHeight);
        }
        void DrawLimb(Transform upper, Transform mid, Transform lower, SoftEllipse upperHigh, SoftEllipse upperLow, SoftEllipse midHigh, SoftEllipse midLow, float upperHighY, float upperLowY, float midHighY, float midLowY, float eyeCenterHeight, bool mirror = false)
        {
            Quaternion worldToAnimator = Quaternion.Inverse(animator.transform.rotation);

            Vector3 femurLf = mid.position - upper.position;
            GuessLimbAxis(worldToAnimator * femurLf, worldToAnimator * upper.rotation, out Vector3 fwdLegLf, out Vector3 upLegLf);
            Vector3 femurLfFwd = upper.rotation * fwdLegLf;
            Vector3 femurLfUp = upper.rotation * upLegLf;
            Vector3 femurLfRt = Vector3.Cross(femurLfUp, femurLfFwd);


            Vector3 shinLf = lower.position - mid.position;
            GuessLimbAxis(worldToAnimator * shinLf, worldToAnimator * mid.rotation, out fwdLegLf, out upLegLf);
            Vector3 shinLfFwd = mid.rotation * fwdLegLf;
            Vector3 shinLfUp = mid.rotation * upLegLf;
            Vector3 shinLfRt = Vector3.Cross(shinLfUp, shinLfFwd);

            Vector3 kneeUp = Vector3.LerpUnclamped(femurLfUp, shinLfUp, .5f);
            Vector3 kneeRt = Vector3.LerpUnclamped(femurLfRt, shinLfRt, .5f);


            Vector3 thighLfWorld = Vector3.Lerp(upper.position, mid.position, upperHighY);
            DrawHexaRing(thighLfWorld, femurLfUp, femurLfRt, upperHigh, eyeCenterHeight, mirror);
            Vector3 kneeLfWorld = Vector3.Lerp(upper.position, mid.position, upperLowY);
            DrawHexaRing(kneeLfWorld, kneeUp, kneeRt, upperLow, eyeCenterHeight, mirror);
            DrawRingConnnection(thighLfWorld, kneeLfWorld, femurLfUp, femurLfRt, kneeUp, kneeRt, upperHigh, upperLow, eyeCenterHeight, mirror);


            Vector3 calfLfWorld = Vector3.Lerp(mid.position, lower.position, midHighY);
            DrawHexaRing(calfLfWorld, shinLfUp, shinLfRt, midHigh, eyeCenterHeight, mirror);
            Vector3 ankleLfWorld = Vector3.Lerp(mid.position, lower.position, midLowY);
            DrawHexaRing(ankleLfWorld, shinLfUp, shinLfRt, midLow, eyeCenterHeight, mirror);
            DrawRingConnnection(calfLfWorld, ankleLfWorld, shinLfUp, shinLfRt, shinLfUp, shinLfRt, midHigh, midLow, eyeCenterHeight, mirror);


            DrawRingConnnection(kneeLfWorld, calfLfWorld, kneeUp, kneeRt, shinLfUp, shinLfRt, upperLow, midHigh, eyeCenterHeight, mirror);
        }
        void DrawHexaRing(Vector3 worldPos, Vector3 fwd, Vector3 right, SoftEllipse ellipse, float eyeHeight, bool mirror = false)
        {


            float radiusFwd = (ellipse.ZRadius + ellipse.ZBias * ellipse.ZRadius) * eyeHeight;
            float radiusBack = (-ellipse.ZRadius + ellipse.ZBias * ellipse.ZRadius) * eyeHeight;
            float radiusLeft = (-ellipse.XRadius + ellipse.XBias * ellipse.XRadius) * eyeHeight;
            float radiusRight = (ellipse.XRadius + ellipse.XBias * ellipse.XRadius) * eyeHeight;



            if (mirror)
            {
                radiusLeft = -(-ellipse.XRadius + ellipse.XBias * ellipse.XRadius) * eyeHeight;
                radiusRight = -(ellipse.XRadius + ellipse.XBias * ellipse.XRadius) * eyeHeight;

            }

            Vector3 ellipseFwd = radiusFwd * fwd;
            Vector3 ellipseBack = radiusBack * fwd;
            Vector3 ellipseLeft = radiusLeft * right;
            Vector3 ellipseRight = radiusRight * right;





            Gizmos.DrawLine(ellipseRight * .383f + ellipseFwd * .924f + worldPos, ellipseFwd + worldPos);
            Gizmos.DrawLine(ellipseFwd + worldPos, ellipseFwd * .924f + ellipseLeft * .383f + worldPos);
            Gizmos.DrawLine(ellipseFwd * .924f + ellipseLeft * .383f + worldPos, ellipseFwd * .383f + ellipseLeft * .924f + worldPos);
            Gizmos.DrawLine(ellipseFwd * .383f + ellipseLeft * .924f + worldPos, ellipseLeft + worldPos);
            Gizmos.DrawLine(ellipseLeft + worldPos, ellipseLeft * .924f + ellipseBack * .383f + worldPos);
            Gizmos.DrawLine(ellipseLeft * .924f + ellipseBack * .383f + worldPos, ellipseLeft * .383f + ellipseBack * .924f + worldPos);
            Gizmos.DrawLine(ellipseBack + worldPos, ellipseBack * .924f + ellipseRight * .383f + worldPos);
            Gizmos.DrawLine(ellipseLeft * .383f + ellipseBack * .924f + worldPos, ellipseBack + worldPos);
            Gizmos.DrawLine(ellipseBack * .924f + ellipseRight * .383f + worldPos, ellipseBack * .383f + ellipseRight * .924f + worldPos);
            Gizmos.DrawLine(ellipseRight + worldPos, ellipseRight * .924f + ellipseFwd * .383f + worldPos);
            Gizmos.DrawLine(ellipseBack * .383f + ellipseRight * .924f + worldPos, ellipseRight + worldPos);
            Gizmos.DrawLine(ellipseRight * .924f + ellipseFwd * .383f + worldPos, ellipseRight * .383f + ellipseFwd * .924f + worldPos);

        }
        void DrawRingConnnection(Vector3 fromWorldPos, Vector3 toWorldPos, Vector3 fwd, Vector3 right, Vector3 fwd2, Vector3 right2, SoftEllipse fromEllipse, SoftEllipse toEllipse, float eyeHeight, bool mirror = false)
        {
            float radiusFwd = (fromEllipse.ZRadius + fromEllipse.ZBias * fromEllipse.ZRadius) * eyeHeight;
            float radiusBack = (-fromEllipse.ZRadius + fromEllipse.ZBias * fromEllipse.ZRadius) * eyeHeight;
            float radiusLeft = (-fromEllipse.XRadius + fromEllipse.XBias * fromEllipse.XRadius) * eyeHeight;
            float radiusRight = (fromEllipse.XRadius + fromEllipse.XBias * fromEllipse.XRadius) * eyeHeight;

            if (mirror)
            {
                radiusLeft = -(-fromEllipse.XRadius + fromEllipse.XBias * fromEllipse.XRadius) * eyeHeight;
                radiusRight = -(fromEllipse.XRadius + fromEllipse.XBias * fromEllipse.XRadius) * eyeHeight;

            }


            Vector3 ellipseFwd = radiusFwd * fwd;
            Vector3 ellipseBack = radiusBack * fwd;
            Vector3 ellipseLeft = radiusLeft * right;
            Vector3 ellipseRight = radiusRight * right;

            float radius2Fwd = (toEllipse.ZRadius + toEllipse.ZBias * toEllipse.ZRadius) * eyeHeight;
            float radius2Back = (-toEllipse.ZRadius + toEllipse.ZBias * toEllipse.ZRadius) * eyeHeight;
            float radius2Left = (-toEllipse.XRadius + toEllipse.XBias * toEllipse.XRadius) * eyeHeight;
            float radius2Right = (toEllipse.XRadius + toEllipse.XBias * toEllipse.XRadius) * eyeHeight;

            if (mirror)
            {
                radius2Left = -(-toEllipse.XRadius + toEllipse.XBias * toEllipse.XRadius) * eyeHeight;
                radius2Right = -(toEllipse.XRadius + toEllipse.XBias * toEllipse.XRadius) * eyeHeight;

            }


            Vector3 ellipse2Fwd = radius2Fwd * fwd2;
            Vector3 ellipse2Back = radius2Back * fwd2;
            Vector3 ellipse2Left = radius2Left * right2;
            Vector3 ellipse2Right = radius2Right * right2;

            Gizmos.DrawLine(ellipseFwd + fromWorldPos, ellipse2Fwd + toWorldPos);
            Gizmos.DrawLine(ellipseFwd * .707f + ellipseLeft * .707f + fromWorldPos, ellipse2Fwd * .707f + ellipse2Left * .707f + toWorldPos);
            Gizmos.DrawLine(ellipseLeft + fromWorldPos, ellipse2Left + toWorldPos);
            Gizmos.DrawLine(ellipseLeft * .707f + ellipseBack * .707f + fromWorldPos, ellipse2Left * .707f + ellipse2Back * .707f + toWorldPos);
            Gizmos.DrawLine(ellipseBack + fromWorldPos, ellipse2Back + toWorldPos);
            Gizmos.DrawLine(ellipseBack * .707f + ellipseRight * .707f + fromWorldPos, ellipse2Back * .707f + ellipse2Right * .707f + toWorldPos);
            Gizmos.DrawLine(ellipseRight + fromWorldPos, ellipse2Right + toWorldPos);
            Gizmos.DrawLine(ellipseRight * .707f + ellipseFwd * .707f + fromWorldPos, ellipse2Right * .707f + ellipse2Fwd * .707f + toWorldPos);
        }
        void DrawBulge(SoftBulge bulge, float bulgeYTop, float bulgeYBot, Vector3 sternumWorld, Vector3 hipsWorld, Vector3 fwd, Vector3 right, AnimationCurve spineZ, AnimationCurve ellipseX, AnimationCurve ellipseZ, AnimationCurve ellipseNegZ, float eyeCenterHeight, bool mirror = false, bool onBack = false)
        {
            float onBackMult = onBack ? -1f : 1f;


            float mirrorMult = mirror ? -1f : 1f;
            float apexYAbs = Mathf.LerpUnclamped(bulgeYTop, bulgeYBot, bulge.apexY);
            float topToBotDiff = bulgeYBot - bulgeYTop;
            float minY = apexYAbs - (topToBotDiff * bulge.apexY) * bulge.upperY;
            float maxY = apexYAbs + (topToBotDiff * (1f - bulge.apexY)) * bulge.lowerY;



            Vector3 up = Vector3.Cross(fwd, right);

            Vector3 highWorld = minY < 1f ? Vector3.LerpUnclamped(sternumWorld, hipsWorld, minY) + spineZ.Evaluate(minY) * eyeCenterHeight * fwd : -_crotchBottom * eyeCenterHeight * (minY - 1f) * up + hipsWorld;
            Vector3 apexWorld = apexYAbs < 1f ? Vector3.LerpUnclamped(sternumWorld, hipsWorld, apexYAbs) + spineZ.Evaluate(apexYAbs) * eyeCenterHeight * fwd : -_crotchBottom * eyeCenterHeight * (apexYAbs - 1f) * up + hipsWorld;
            Vector3 lowWorld = maxY < 1f ? Vector3.LerpUnclamped(sternumWorld, hipsWorld, maxY) + spineZ.Evaluate(maxY) * eyeCenterHeight * fwd : -_crotchBottom * eyeCenterHeight * (maxY - 1f) * up + hipsWorld;

            Vector3 highMidWorld = Vector3.LerpUnclamped(highWorld, apexWorld, .5f);
            Vector3 lowMidWorld = Vector3.LerpUnclamped(apexWorld, lowWorld, .5f);

            Vector3 highMidMidWorld = Vector3.LerpUnclamped(highMidWorld, apexWorld, .5f);
            Vector3 lowMidMidWorld = Vector3.LerpUnclamped(apexWorld, lowMidWorld, .5f);
            Vector3 highHighMidWorld = Vector3.LerpUnclamped(highMidWorld, highWorld, .5f);
            Vector3 lowLowMidWorld = Vector3.LerpUnclamped(lowWorld, lowMidWorld, .5f);

            float minX = bulge.apexX - bulge.innerX;

            float innerRadians = minX * Mathf.Deg2Rad;
            float innerMidRadians = Mathf.LerpUnclamped(bulge.apexX, minX, .5f) * Mathf.Deg2Rad;
            float apexRadians = bulge.apexX * Mathf.Deg2Rad;
            float outerMidRadians = Mathf.LerpUnclamped(bulge.apexX, (bulge.apexX + bulge.outerX), .5f) * Mathf.Deg2Rad;
            float outerRadians = (bulge.apexX + bulge.outerX) * Mathf.Deg2Rad;




            float innerCos = Mathf.Cos(innerRadians) * onBackMult;
            float innerSin = Mathf.Sqrt(1f - (innerCos * innerCos)) * Mathf.Sign(innerRadians) * mirrorMult;
            float innerMidCos = Mathf.Cos(innerMidRadians) * onBackMult;
            float innerMidSin = Mathf.Sqrt(1f - (innerMidCos * innerMidCos)) * Mathf.Sign(innerMidRadians) * mirrorMult;
            float apexCos = Mathf.Cos(apexRadians) * onBackMult;
            float apexSin = Mathf.Sqrt(1f - (apexCos * apexCos)) * mirrorMult;

            float outerMidCos = Mathf.Cos(outerMidRadians) * onBackMult;
            float outerMidSin = Mathf.Sqrt(1f - (outerMidCos * outerMidCos)) * mirrorMult;
            float outerCos = Mathf.Cos(outerRadians) * onBackMult;
            float outerSin = Mathf.Sqrt(1f - (outerCos * outerCos)) * mirrorMult;


            float lowInnerSin = Mathf.LerpUnclamped(innerSin, apexSin, bulge.roundLowerInner);
            float lowInnerCos = Mathf.Sqrt(1f - (lowInnerSin * lowInnerSin)) * onBackMult;
            float lowMidInnerSin = Mathf.LerpUnclamped(innerSin, apexSin, bulge.roundLowerInner * .25f);
            float lowMidInnerCos = Mathf.Sqrt(1f - (lowMidInnerSin * lowMidInnerSin)) * onBackMult;
            float lowInnerMidSin = Mathf.LerpUnclamped(innerMidSin, apexSin, bulge.roundLowerInner);
            float lowInnerMidCos = Mathf.Sqrt(1f - (lowInnerMidSin * lowInnerMidSin)) * onBackMult;
            float lowMidInnerMidSin = Mathf.LerpUnclamped(innerMidSin, apexSin, bulge.roundLowerInner * .25f);
            float lowMidInnerMidCos = Mathf.Sqrt(1f - (lowMidInnerMidSin * lowMidInnerMidSin)) * onBackMult;

            float highInnerSin = Mathf.LerpUnclamped(innerSin, apexSin, bulge.roundUpperInner);
            float highInnerCos = Mathf.Sqrt(1f - (highInnerSin * highInnerSin)) * onBackMult;
            float highMidInnerSin = Mathf.LerpUnclamped(innerSin, apexSin, bulge.roundUpperInner * .25f);
            float highMidInnerCos = Mathf.Sqrt(1f - (highMidInnerSin * highMidInnerSin)) * onBackMult;
            float highInnerMidSin = Mathf.LerpUnclamped(innerMidSin, apexSin, bulge.roundUpperInner);
            float highInnerMidCos = Mathf.Sqrt(1f - (highInnerMidSin * highInnerMidSin)) * onBackMult;
            float highMidInnerMidSin = Mathf.LerpUnclamped(innerMidSin, apexSin, bulge.roundUpperInner * .25f);
            float highMidInnerMidCos = Mathf.Sqrt(1f - (highMidInnerMidSin * highMidInnerMidSin)) * onBackMult;

            if (minX < 0f)
            {
                if (innerSin * mirrorMult < 0f)
                {
                    innerSin = 0f;
                    innerCos = onBackMult;
                }
                if (innerMidSin * mirrorMult < 0f)
                {
                    innerMidSin = 0f;
                    innerMidCos = onBackMult;
                }
                if (lowInnerSin * mirrorMult < 0f)
                {
                    lowInnerSin = 0f;
                    lowInnerCos = onBackMult;
                }
                if (lowMidInnerSin * mirrorMult < 0f)
                {
                    lowMidInnerSin = 0f;
                    lowMidInnerCos = onBackMult;
                }
                if (lowInnerMidSin * mirrorMult < 0f)
                {
                    lowInnerMidSin = 0f;
                    lowInnerMidCos = onBackMult;
                }
                if (lowMidInnerMidSin * mirrorMult < 0f)
                {
                    lowMidInnerMidSin = 0f;
                    lowMidInnerMidCos = onBackMult;
                }
                if (highInnerSin * mirrorMult < 0f)
                {
                    highInnerSin = 0f;
                    highInnerCos = onBackMult;
                }
                if (highMidInnerSin * mirrorMult < 0f)
                {
                    highMidInnerSin = 0f;
                    highMidInnerCos = onBackMult;
                }
                if (highInnerMidSin * mirrorMult < 0f)
                {
                    highInnerMidSin = 0f;
                    highInnerMidCos = onBackMult;
                }
                if (highMidInnerMidSin * mirrorMult < 0f)
                {
                    highMidInnerMidSin = 0f;
                    highMidInnerMidCos = onBackMult;
                }
            }

            float lowOuterCos = Mathf.LerpUnclamped(outerCos, apexCos, bulge.roundLowerOuter);
            float lowOuterSin = Mathf.Sqrt(1f - (lowOuterCos * lowOuterCos)) * mirrorMult;
            float lowMidOuterCos = Mathf.LerpUnclamped(outerCos, apexCos, bulge.roundLowerOuter * .25f);
            float lowMidOuterSin = Mathf.Sqrt(1f - (lowMidOuterCos * lowMidOuterCos)) * mirrorMult;
            float lowOuterMidCos = Mathf.LerpUnclamped(outerMidCos, apexCos, bulge.roundLowerOuter);
            float lowOuterMidSin = Mathf.Sqrt(1f - (lowOuterMidCos * lowOuterMidCos)) * mirrorMult;
            float lowMidOuterMidCos = Mathf.LerpUnclamped(outerMidCos, apexCos, bulge.roundLowerOuter * .25f);
            float lowMidOuterMidSin = Mathf.Sqrt(1f - (lowMidOuterMidCos * lowMidOuterMidCos)) * mirrorMult;

            float highOuterCos = Mathf.LerpUnclamped(outerCos, apexCos, bulge.roundUpperOuter);
            float highOuterSin = Mathf.Sqrt(1f - (highOuterCos * highOuterCos)) * mirrorMult;
            float highMidOuterCos = Mathf.LerpUnclamped(outerCos, apexCos, bulge.roundUpperOuter * .25f);
            float highMidOuterSin = Mathf.Sqrt(1f - (highMidOuterCos * highMidOuterCos)) * mirrorMult;
            float highOuterMidCos = Mathf.LerpUnclamped(outerMidCos, apexCos, bulge.roundUpperOuter);
            float highOuterMidSin = Mathf.Sqrt(1f - (highOuterMidCos * highOuterMidCos)) * mirrorMult;
            float highMidOuterMidCos = Mathf.LerpUnclamped(outerMidCos, apexCos, bulge.roundUpperOuter * .25f);
            float highMidOuterMidSin = Mathf.Sqrt(1f - (highMidOuterMidCos * highMidOuterMidCos)) * mirrorMult;

            Vector3 highInnerCoord = (GetSoftTorso(minY, new Vector2(highInnerSin, highInnerCos), ellipseX, ellipseZ, ellipseNegZ, out Vector3 softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highInner = highInnerCoord.x * right + highInnerCoord.z * fwd + highInnerCoord.y * up;
            Vector3 highInnerMidCoord = (GetSoftTorso(minY, new Vector2(highInnerMidSin, highInnerMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highInnerMid = highInnerMidCoord.x * right + highInnerMidCoord.z * fwd + highInnerMidCoord.y * up;
            Vector3 highApexCoord = (GetSoftTorso(minY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highApex = highApexCoord.x * right + highApexCoord.z * fwd + highApexCoord.y * up;
            Vector3 highOuterMidCoord = (GetSoftTorso(minY, new Vector2(highOuterMidSin, highOuterMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highOuterMid = highOuterMidCoord.x * right + highOuterMidCoord.z * fwd + highOuterMidCoord.y * up;
            Vector3 highOuterCoord = (GetSoftTorso(minY, new Vector2(highOuterSin, highOuterCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highOuter = highOuterCoord.x * right + highOuterCoord.z * fwd + highOuterCoord.y * up;

            float highMidY = Mathf.LerpUnclamped(minY, apexYAbs, .5f);
            Vector3 highMidInnerCoord = (GetSoftTorso(highMidY, new Vector2(highMidInnerSin, highMidInnerCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highMidInner = highMidInnerCoord.x * right + highMidInnerCoord.z * fwd + highMidInnerCoord.y * up;
            Vector3 highMidInnerMidCoord = (GetSoftTorso(highMidY, new Vector2(highMidInnerMidSin, highMidInnerMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highMidInnerMid = highMidInnerMidCoord.x * right + highMidInnerMidCoord.z * fwd + highMidInnerMidCoord.y * up;
            Vector3 highMidApexCoord = (GetSoftTorso(highMidY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highMidApex = highMidApexCoord.x * right + highMidApexCoord.z * fwd + highMidApexCoord.y * up;
            Vector3 highMidOuterMidCoord = (GetSoftTorso(highMidY, new Vector2(highMidOuterMidSin, highMidOuterMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highMidOuterMid = highMidOuterMidCoord.x * right + highMidOuterMidCoord.z * fwd + highMidOuterMidCoord.y * up;
            Vector3 highMidOuterCoord = (GetSoftTorso(highMidY, new Vector2(highMidOuterSin, highMidOuterCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highMidOuter = highMidOuterCoord.x * right + highMidOuterCoord.z * fwd + highMidOuterCoord.y * up;

            float highHighMidY = Mathf.LerpUnclamped(highMidY, minY, .5f);
            Vector3 highHighMidApexCoord = (GetSoftTorso(highHighMidY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highHighMidApex = highHighMidApexCoord.x * right + highHighMidApexCoord.z * fwd + highHighMidApexCoord.y * up;
            float highMidMidY = Mathf.LerpUnclamped(highMidY, apexYAbs, .5f);
            Vector3 highMidMidApexCoord = (GetSoftTorso(highMidMidY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 highMidMidApex = highMidMidApexCoord.x * right + highMidMidApexCoord.z * fwd + highMidMidApexCoord.y * up;

            Vector3 apexInnerCoord = (GetSoftTorso(apexYAbs, new Vector2(innerSin, innerCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 apexInner = apexInnerCoord.x * right + apexInnerCoord.z * fwd + apexInnerCoord.y * up;
            Vector3 apexInnerMidCoord = (GetSoftTorso(apexYAbs, new Vector2(innerMidSin, innerMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 apexInnerMid = apexInnerMidCoord.x * right + apexInnerMidCoord.z * fwd + apexInnerMidCoord.y * up;
            Vector3 apexCoord = (GetSoftTorso(apexYAbs, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 apex = apexCoord.x * right + apexCoord.z * fwd + apexCoord.y * up;
            Vector3 apexOuterMidCoord = (GetSoftTorso(apexYAbs, new Vector2(outerMidSin, outerMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 apexOuterMid = apexOuterMidCoord.x * right + apexOuterMidCoord.z * fwd + apexOuterMidCoord.y * up;
            Vector3 apexOuterCoord = (GetSoftTorso(apexYAbs, new Vector2(outerSin, outerCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 apexOuter = apexOuterCoord.x * right + apexOuterCoord.z * fwd + apexOuterCoord.y * up;

            float lowMidY = Mathf.LerpUnclamped(apexYAbs, maxY, .5f);
            Vector3 lowMidInnerCoord = (GetSoftTorso(lowMidY, new Vector2(lowMidInnerSin, lowMidInnerCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowMidInner = lowMidInnerCoord.x * right + lowMidInnerCoord.z * fwd + lowMidInnerCoord.y * up;
            Vector3 lowMidInnerMidCoord = (GetSoftTorso(lowMidY, new Vector2(lowMidInnerMidSin, lowMidInnerMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowMidInnerMid = lowMidInnerMidCoord.x * right + lowMidInnerMidCoord.z * fwd + lowMidInnerMidCoord.y * up;
            Vector3 lowMidApexCoord = (GetSoftTorso(lowMidY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowMidApex = lowMidApexCoord.x * right + lowMidApexCoord.z * fwd + lowMidApexCoord.y * up;
            Vector3 lowMidOuterMidCoord = (GetSoftTorso(lowMidY, new Vector2(lowMidOuterMidSin, lowMidOuterMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowMidOuterMid = lowMidOuterMidCoord.x * right + lowMidOuterMidCoord.z * fwd + lowMidOuterMidCoord.y * up;
            Vector3 lowMidOuterCoord = (GetSoftTorso(lowMidY, new Vector2(lowMidOuterSin, lowMidOuterCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowMidOuter = lowMidOuterCoord.x * right + lowMidOuterCoord.z * fwd + lowMidOuterCoord.y * up;

            float lowMidMidY = Mathf.LerpUnclamped(apexYAbs, lowMidY, .5f);
            Vector3 lowMidMidApexCoord = (GetSoftTorso(lowMidMidY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowMidMidApex = lowMidMidApexCoord.x * right + lowMidMidApexCoord.z * fwd + lowMidMidApexCoord.y * up;
            float lowLowMidY = Mathf.LerpUnclamped(maxY, lowMidY, .5f);
            Vector3 lowLowMidApexCoord = (GetSoftTorso(lowLowMidY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowLowMidApex = lowLowMidApexCoord.x * right + lowLowMidApexCoord.z * fwd + lowLowMidApexCoord.y * up;

            Vector3 lowInnerCoord = (GetSoftTorso(maxY, new Vector2(lowInnerSin, lowInnerCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowInner = lowInnerCoord.x * right + lowInnerCoord.z * fwd + lowInnerCoord.y * up;
            Vector3 lowInnerMidCoord = (GetSoftTorso(maxY, new Vector2(lowInnerMidSin, lowInnerMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowInnerMid = lowInnerMidCoord.x * right + lowInnerMidCoord.z * fwd + lowInnerMidCoord.y * up;
            Vector3 lowApexCoord = (GetSoftTorso(maxY, new Vector2(apexSin, apexCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowApex = lowApexCoord.x * right + lowApexCoord.z * fwd + lowApexCoord.y * up;
            Vector3 lowOuterMidCoord = (GetSoftTorso(maxY, new Vector2(lowOuterMidSin, lowOuterMidCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowOuterMid = lowOuterMidCoord.x * right + lowOuterMidCoord.z * fwd + lowOuterMidCoord.y * up;
            Vector3 lowOuterCoord = (GetSoftTorso(maxY, new Vector2(lowOuterSin, lowOuterCos), ellipseX, ellipseZ, ellipseNegZ, out softDisplace) + softDisplace) * eyeCenterHeight;
            Vector3 lowOuter = lowOuterCoord.x * right + lowOuterCoord.z * fwd + lowOuterCoord.y * up;

            Color apexColor = (bulge.rigidity < .5f) ? Color.LerpUnclamped(new Color(1f, 0f, 0f, 1f), new Color(.2f, 1f, 0f, 1f), bulge.rigidity * 2f) : Color.LerpUnclamped(new Color(.2f, 1f, 0f, 1f), new Color(0f, .5f, 1f, 1f), (bulge.rigidity - .5f) * 2f);
            Color halfwayColor = Color.LerpUnclamped(new Color(.2f, 1f, 0f, 1f), new Color(0f, .5f, 1f, 1f), bulge.rigidity);
            Color baseColor = new Color(0f, .5f, 1f, 1f);
            Gizmos.color = apexColor;


            Gizmos.color = Color.LerpUnclamped(halfwayColor, apexColor, .75f);
            Gizmos.DrawLine(apex + apexWorld, highMidMidApex + highMidMidWorld);
            Gizmos.DrawLine(apex + apexWorld, lowMidMidApex + lowMidMidWorld);

            Gizmos.color = Color.LerpUnclamped(halfwayColor, apexColor, .5f);
            Gizmos.DrawLine(highMidMidApex + highMidMidWorld, highMidApex + highMidWorld);
            Gizmos.DrawLine(apex + apexWorld, apexInnerMid + apexWorld);
            Gizmos.DrawLine(lowMidMidApex + lowMidMidWorld, lowMidApex + lowMidWorld);
            Gizmos.DrawLine(apex + apexWorld, apexOuterMid + apexWorld);

            Gizmos.color = halfwayColor;
            Gizmos.DrawLine(highMidApex + highMidWorld, highMidInnerMid + highMidWorld);
            Gizmos.DrawLine(highMidInnerMid + highMidWorld, apexInnerMid + apexWorld);
            Gizmos.DrawLine(apexInnerMid + apexWorld, lowMidInnerMid + lowMidWorld);
            Gizmos.DrawLine(lowMidInnerMid + lowMidWorld, lowMidApex + lowMidWorld);
            Gizmos.DrawLine(lowMidApex + lowMidWorld, lowMidOuterMid + lowMidWorld);
            Gizmos.DrawLine(lowMidOuterMid + lowMidWorld, apexOuterMid + apexWorld);
            Gizmos.DrawLine(apexOuterMid + apexWorld, highMidOuterMid + highMidWorld);
            Gizmos.DrawLine(highMidOuterMid + highMidWorld, highMidApex + highMidWorld);

            Gizmos.color = Color.LerpUnclamped(baseColor, halfwayColor, .5f);
            Gizmos.DrawLine(highMidApex + highMidWorld, highHighMidApex + highHighMidWorld);
            Gizmos.DrawLine(highMidInnerMid + highMidWorld, highInnerMid + highWorld);
            Gizmos.DrawLine(highMidInnerMid + highMidWorld, highMidInner + highMidWorld);
            Gizmos.DrawLine(apexInnerMid + apexWorld, apexInner + apexWorld);
            Gizmos.DrawLine(lowMidInnerMid + lowMidWorld, lowMidInner + lowMidWorld);
            Gizmos.DrawLine(lowMidInnerMid + lowMidWorld, lowInnerMid + lowWorld);
            Gizmos.DrawLine(lowMidApex + lowMidWorld, lowLowMidApex + lowLowMidWorld);
            Gizmos.DrawLine(lowMidOuterMid + lowMidWorld, lowOuterMid + lowWorld);
            Gizmos.DrawLine(lowMidOuterMid + lowMidWorld, lowMidOuter + lowMidWorld);
            Gizmos.DrawLine(apexOuterMid + apexWorld, apexOuter + apexWorld);
            Gizmos.DrawLine(highMidOuterMid + highMidWorld, highMidOuter + highMidWorld);
            Gizmos.DrawLine(highMidOuterMid + highMidWorld, highOuterMid + highWorld);

            Gizmos.color = Color.LerpUnclamped(baseColor, halfwayColor, .25f);
            Gizmos.DrawLine(highHighMidApex + highHighMidWorld, highApex + highWorld);
            Gizmos.DrawLine(lowLowMidApex + lowLowMidWorld, lowApex + lowWorld);

            Gizmos.color = baseColor;
            Gizmos.DrawLine(highInner + highWorld, highInnerMid + highWorld);
            Gizmos.DrawLine(highInnerMid + highWorld, highApex + highWorld);
            Gizmos.DrawLine(highApex + highWorld, highOuterMid + highWorld);
            Gizmos.DrawLine(highOuterMid + highWorld, highOuter + highWorld);
            Gizmos.DrawLine(highOuter + highWorld, highMidOuter + highMidWorld);
            Gizmos.DrawLine(highMidOuter + highMidWorld, apexOuter + apexWorld);
            Gizmos.DrawLine(apexOuter + apexWorld, lowMidOuter + lowMidWorld);
            Gizmos.DrawLine(lowMidOuter + lowMidWorld, lowOuter + lowWorld);
            Gizmos.DrawLine(lowOuter + lowWorld, lowOuterMid + lowWorld);
            Gizmos.DrawLine(lowOuterMid + lowWorld, lowApex + lowWorld);
            Gizmos.DrawLine(lowApex + lowWorld, lowInnerMid + lowWorld);
            Gizmos.DrawLine(lowInnerMid + lowWorld, lowInner + lowWorld);
            Gizmos.DrawLine(lowInner + lowWorld, lowMidInner + lowMidWorld);
            Gizmos.DrawLine(lowMidInner + lowMidWorld, apexInner + apexWorld);
            Gizmos.DrawLine(apexInner + apexWorld, highMidInner + highMidWorld);
            Gizmos.DrawLine(highMidInner + highMidWorld, highInner + highWorld);
        }
        private SimpleTransform GetSpineInWorldForGizmos(float yPerc, float eyeCenterHeight, SimpleTransform head, SimpleTransform sternum, SimpleTransform hipCenter)
        {

            SimpleTransform nearestVrSpine = sternum;

            if (yPerc >= 2f)
            {
                nearestVrSpine = hipCenter;
                nearestVrSpine.position -= _crotchBottom * eyeCenterHeight * hipCenter.up;
            }
            else if (yPerc > 1f)
            {
                Vector3 pelvisDown = -hipCenter.up;
                float crotchMeters = _crotchBottom * eyeCenterHeight;
                nearestVrSpine = hipCenter;
                nearestVrSpine.position += (yPerc - 1f) * crotchMeters * pelvisDown;
            }
            else if (yPerc > 0f)
            {
                nearestVrSpine = SimpleTransform.Lerp(nearestVrSpine, hipCenter, yPerc);
            }
            else if (yPerc > -1f)
            {
                nearestVrSpine = SimpleTransform.Lerp(head, nearestVrSpine, yPerc + 1f);
            }







            else
            {
                nearestVrSpine = head;
                nearestVrSpine.position += _headTop * eyeCenterHeight * head.up;
            }

            return nearestVrSpine;
        }
    }
}
