# Generated by Django 5.0.7 on 2024-07-28 04:41

from django.db import migrations


class Migration(migrations.Migration):

    dependencies = [
        ('tracker', '0002_expense'),
    ]

    operations = [
        migrations.RenameField(
            model_name='expense',
            old_name='item',
            new_name='item_name',
        ),
    ]
