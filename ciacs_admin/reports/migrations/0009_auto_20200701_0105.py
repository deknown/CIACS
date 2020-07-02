# Generated by Django 3.0.7 on 2020-06-30 21:05

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('reports', '0008_auto_20200628_1842'),
    ]

    operations = [
        migrations.AlterField(
            model_name='cpus',
            name='bits',
            field=models.IntegerField(blank=True, max_length=8, null=True),
        ),
        migrations.AlterField(
            model_name='cpus',
            name='cores',
            field=models.IntegerField(blank=True, max_length=4, null=True),
        ),
        migrations.AlterField(
            model_name='cpus',
            name='manufacturer',
            field=models.CharField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='cpus',
            name='name',
            field=models.CharField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='cpus',
            name='speed',
            field=models.IntegerField(blank=True, max_length=5, null=True),
        ),
        migrations.AlterField(
            model_name='cpus',
            name='threads',
            field=models.IntegerField(blank=True, max_length=5, null=True),
        ),
        migrations.AlterField(
            model_name='disks',
            name='capacity',
            field=models.IntegerField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='disks',
            name='type',
            field=models.CharField(blank=True, max_length=10, null=True),
        ),
        migrations.AlterField(
            model_name='gpus',
            name='capacity',
            field=models.IntegerField(blank=True, max_length=15, null=True),
        ),
        migrations.AlterField(
            model_name='gpus',
            name='manufacturer',
            field=models.CharField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='gpus',
            name='name',
            field=models.CharField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='motherboards',
            name='manufacturer',
            field=models.CharField(blank=True, max_length=250, null=True),
        ),
        migrations.AlterField(
            model_name='motherboards',
            name='name',
            field=models.CharField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='os',
            name='bits',
            field=models.IntegerField(blank=True, max_length=8, null=True),
        ),
        migrations.AlterField(
            model_name='os',
            name='build',
            field=models.CharField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='os',
            name='name',
            field=models.CharField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='rams',
            name='capacity',
            field=models.IntegerField(blank=True, max_length=15, null=True),
        ),
        migrations.AlterField(
            model_name='rams',
            name='manufacturer',
            field=models.CharField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='rams',
            name='name',
            field=models.CharField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='rams',
            name='type',
            field=models.CharField(blank=True, max_length=50, null=True),
        ),
        migrations.AlterField(
            model_name='software',
            name='name',
            field=models.CharField(blank=True, max_length=250, null=True),
        ),
    ]
