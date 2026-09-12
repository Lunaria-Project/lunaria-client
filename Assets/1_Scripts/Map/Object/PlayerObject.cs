using JetBrains.Annotations;
using Spine;
using Spine.Unity.AttachmentTools;
using UnityEngine;

public class PlayerObject : MovableObject
{
    public void Init(Vector2 position)
    {
        InitPositionAndScale(position, new Vector2(0, 86), 0.5f, 1);
    }

    protected void OnEnable()
    {
        UserData.Instance.OnEquippedArtifactChanged -= RefreshArtifactSkin;
        UserData.Instance.OnEquippedArtifactChanged += RefreshArtifactSkin;
    }

    protected void OnDisable()
    {
        UserData.Instance.OnEquippedArtifactChanged -= RefreshArtifactSkin;
    }

    protected override void Update()
    {
        base.Update();
        if (!GlobalManager.Instance.CanPlayerMove()) return;

        var moveUp = Input.GetKey(KeyCode.W);
        var moveDown = Input.GetKey(KeyCode.S);
        var moveRight = Input.GetKey(KeyCode.D);
        var moveLeft = Input.GetKey(KeyCode.A);
        if (moveUp || moveDown || moveRight || moveLeft)
        {
            StopAutoMove();
        }
        if (IsAutoMoving) return;

        var previousMoveDirection = MoveDirection;
        MoveDirection = Vector2.zero;
        if (moveUp && moveDown)
        {
            MoveDirection += previousMoveDirection.y > 0 ? Vector2.up : Vector2.down;
        }
        else if (moveUp)
        {
            MoveDirection += Vector2.up;
        }
        else if (moveDown)
        {
            MoveDirection += Vector2.down;
        }

        if (moveLeft && moveRight)
        {
            MoveDirection += previousMoveDirection.x > 0 ? Vector2.right : Vector2.left;
        }
        else if (moveLeft)
        {
            MoveDirection += Vector2.left;
        }
        else if (moveRight)
        {
            MoveDirection += Vector2.right;
        }

        MoveDirection.Normalize();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MoveDirection *= GameSetting.Instance.SpeedUpRate;
        }
    }

    protected override int GetCharacterDataId()
    {
        return 1000011;
    }

    #region Artifact

    protected override void OnSkeletonAnimationInitialized()
    {
        RefreshArtifactSkin();
    }

    private void RefreshArtifactSkin()
    {
        if (!UseSkeletonAnimation) return;

        var skeleton = SkeletonAnimation.Skeleton;
        var artifactSkin = CreateArtifactSkin(skeleton.Data);

        skeleton.SetSkin(artifactSkin ?? skeleton.Data.DefaultSkin);
        skeleton.SetupPoseSlots();
        SkeletonAnimation.AnimationState.Apply(skeleton);
    }

    [CanBeNull]
    private Skin CreateArtifactSkin(SkeletonData skeletonData)
    {
        var artifactItemId = UserData.Instance.EquippedArtifactId;
        if (artifactItemId == 0) return null;

        var resourceKey = GameData.Instance.GetArtifactData(artifactItemId).ResourceKey;
        if (string.IsNullOrEmpty(resourceKey)) return null;

        var sprite = ResourceManager.Instance.LoadSprite(resourceKey);
        if (sprite == null)
        {
            LogManager.LogError($"[PlayerObject] {resourceKey}: 아티팩트 스파인 이미지를 찾을 수 없습니다.");
            return null;
        }

        var slotData = skeletonData.FindSlot(NameContainer.SpineSlot.Artifact);
        var sourceSkin = skeletonData.FindSkin(NameContainer.SpineSkin.Artifact);
        if (slotData == null || sourceSkin == null)
        {
            LogManager.LogError($"[PlayerObject] {SkeletonAnimation.SkeletonDataAsset.name}: 아티팩트 슬롯 또는 스킨을 찾을 수 없습니다. Slot: {NameContainer.SpineSlot.Artifact}, Skin: {NameContainer.SpineSkin.Artifact}");
            return null;
        }

        var sourceAttachment = sourceSkin.GetAttachment(slotData.Index, NameContainer.SpineAttachment.Artifact);
        if (sourceAttachment == null)
        {
            LogManager.LogError($"[PlayerObject] {SkeletonAnimation.SkeletonDataAsset.name}: 아티팩트 어태치먼트를 찾을 수 없습니다. Attachment: {NameContainer.SpineAttachment.Artifact}");
            return null;
        }

        // SkeletonDataAsset은 캐싱되어 공유되므로 원본 어태치먼트를 수정하면 같은 스켈레톤을 쓰는 모든 캐릭터가 함께 바뀐다.
        var replacedAttachment = sourceAttachment.Copy();
        var sourceMaterial = SkeletonAnimation.SkeletonDataAsset.atlasAssets.GetAt(0).PrimaryMaterial;
        replacedAttachment.SetRegion(sprite, sourceMaterial, useOriginalRegionScale: true);

        // 교체하지 않는 파츠는 Skeleton.GetAttachment의 DefaultSkin 폴백으로 원본을 그대로 사용한다.
        var artifactSkin = new Skin(resourceKey);
        artifactSkin.SetAttachment(slotData.Index, NameContainer.SpineAttachment.Artifact, replacedAttachment);
        return artifactSkin;
    }

    #endregion
}