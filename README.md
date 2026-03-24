# Nazarick Escape (Unity 2D)

Ниже — версия платформера **для Unity** (URP не обязателен), а не веб-канвас.

## Что реализовано
- 2D-платформер с управлением `A/D` (или стрелки), прыжком (`Space/W/Up`), рывком (`Shift`), рестартом (`R`).
- HP, i-frames после урона, смерть и респавн.
- Сбор рун (collectibles), выход через портал только после сбора всех рун.
- Переход между уровнями и экран победы.
- Патрулирующие враги с уроном.
- Простая камера, следующая за игроком.

## Быстрый старт в Unity (2022.3+)
1. Создай 2D проект.
2. Скопируй папку `Assets/Scripts` в проект.
3. Создай сцену `Level1`.
4. Добавь объекты:
   - `GameManager` (пустой объект) + компонент `GameManager`.
   - `Player` (Sprite + `Rigidbody2D` + `CapsuleCollider2D` + `PlayerController2D`).
   - `GroundCheck` как дочерний объект игрока под ногами.
   - `Camera` + `CameraFollow2D`.
   - Платформы с `BoxCollider2D` и слоем `Ground`.
   - Руны: любой спрайт + `CircleCollider2D` (`Is Trigger`) + `GemCollectible`.
   - Враги: спрайт + `BoxCollider2D` (`Is Trigger`) + `PatrolEnemy`.
   - Портал: спрайт + `BoxCollider2D` (`Is Trigger`) + `LevelExit`.
5. В `GameManager` укажи:
   - `Player`, `Spawn Point`, `Message Label` (TMP), `HP Label` (TMP), `Gems Label` (TMP).
   - `Level Scene Names`: `Level1`, `Level2`.
6. Для `Level2` создай вторую сцену и добавь её в Build Settings.

## Примечания
- Для UI используется TextMeshPro (`TMP_Text`).
- Можно расширить до metroidvania: способности через ScriptableObject, карту зон, сохранения.
