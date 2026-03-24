# Nazarick Escape (Unity 2D) — готово к сборке

Сделал Unity-версию с **примитивной визуализацией** (цветные спрайты/коллайдеры), базовым платформингом и тремя типами атак.

## Что есть сейчас
- Платформер: бег, прыжок, двойной прыжок, рывок.
- Боевая система:
  - **Слабая атака** (ЛКМ) — быстрый ближний удар.
  - **Сильная атака** (ПКМ) — медленнее, но выше урон и радиус.
  - **Дальняя атака** (`Q`) — снаряд.
- Враги с HP (их можно убивать), контактный урон по игроку.
- Сбор рун и выход в портал только после сбора всех.
- GameManager с HP/сообщениями/рестартом (`R`).

## Быстрый запуск в Unity 2022.3+
1. Создай 2D проект Unity.
2. Скопируй `Assets/Scripts`.
3. На сцене добавь:
   - `GameManager` + компонент `GameManager`.
   - `Player`:
     - `SpriteRenderer` (любой квадратный спрайт),
     - `Rigidbody2D` (Dynamic),
     - `CapsuleCollider2D`,
     - `PlayerController2D`.
   - `GroundCheck` (дочерний объект Player, чуть ниже ног).
   - `Main Camera` + `CameraFollow2D`.
   - Землю/платформы с `BoxCollider2D` на слое `Ground`.
   - Врагов с `SpriteRenderer`, `BoxCollider2D (IsTrigger)`, `PatrolEnemy`.
   - Руны с `SpriteRenderer`, `CircleCollider2D (IsTrigger)`, `GemCollectible`.
   - Портал с `SpriteRenderer`, `BoxCollider2D (IsTrigger)`, `LevelExit`.
4. Для дальней атаки создай prefab снаряда:
   - `Projectile` объект: `SpriteRenderer`, `Rigidbody2D` (Gravity Scale 0), `CircleCollider2D (IsTrigger)`, `Projectile`.
   - Назначь prefab в `PlayerController2D.projectilePrefab`.
5. Для визуала удара создай prefab эффекта:
   - объект со `SpriteRenderer` (квадрат/круг),
   - назначь в `PlayerController2D.hitEffectPrefab`.
6. В `GameManager` назначь `player`, `spawnPoint`, список сцен (`Level1`, `Level2` при необходимости).

## Управление
- `A/D` или стрелки — движение.
- `Space/W/↑` — прыжок.
- `Shift` — рывок.
- `ЛКМ` — слабая атака.
- `ПКМ` — сильная атака.
- `Q` — дальняя атака.
- `R` — перезапуск забега.

## Сборка (когда Unity установлен локально)
В этом контейнере Unity не установлен, поэтому бинарник (`.exe`) здесь собрать нельзя. Локально можно собрать так:
1. Открой проект в Unity.
2. `File -> Build Settings` -> добавь сцены.
3. Выбери платформу (например, Windows) и нажми `Build`.

### Автосборка через CLI (batchmode)
Добавлен скрипт `Assets/Editor/BuildScript.cs` с методами:
- `BuildScript.BuildWindows`
- `BuildScript.BuildLinux`

Пример запуска (Windows билд):
```bash
Unity -batchmode -quit -projectPath "<путь_к_проекту>" -executeMethod BuildScript.BuildWindows -logFile -
```

Пример запуска (Linux билд):
```bash
Unity -batchmode -quit -projectPath "<путь_к_проекту>" -executeMethod BuildScript.BuildLinux -logFile -
```

Готовые файлы попадают в `Builds/<Target>/`.
